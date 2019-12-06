#define WINDOWS
#define OSX
#define LINUX

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Numerics;
using SDL2;
using NuklearDotNet;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;

namespace ProxorEditor.GUI.GUI_Contexts
{
    internal class GL_Texture
    {
        private uint[] glTextureArray = new uint[1] { 0 };

        private OpenGL gl;

        public GL_Texture(OpenGL gl, int W, int H, IntPtr Data)
        {
            this.gl = gl;
            gl.GenTextures(1, glTextureArray);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, TextureName);
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA, W, H, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, Data);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
        }

        ~GL_Texture()
        {
            gl.DeleteTextures(glTextureArray.Length, glTextureArray);
            glTextureArray[0] = 0;
        }

        public uint TextureName
        {
            get { return glTextureArray[0]; }
        }
    }


    internal class GL_GUI_Context : NuklearDeviceTex<GL_Texture>, IFrameBuffered
    {
        private Window window;
        private IntPtr context;
        private OpenGL gl;


        private VertexBufferArray vertexBufferArray;
        private ShaderProgram shaderProgram;

        private float Width, Height;


        private NkVertex[] Verts;
        private ushort[] Inds;

        private Vector2 scale;
        private float[] ortho = new float[16];


        private uint[] framebuffers = new uint[] { 0 };
        private uint[] framebufferTargets = new uint[] { 0 };


        public IntPtr RenderTexture = IntPtr.Zero;

        private const string VERTEX_SHADER = @"
                                                #version 150 core

                                                in vec2 in_Position;
                                                in vec4 in_Color;  
                                                in vec2 in_UV;  

                                                out vec4 pass_Color;
                                                out vec2 pass_UV;

                                                uniform mat4 projectionMatrix;
                                                uniform mat4 viewMatrix;
                                                uniform mat4 modelMatrix;

                                                void main(void) {
	                                                gl_Position = projectionMatrix * vec4(in_Position.xy, 0, 1.0);
	                                                pass_Color = in_Color;
                                                    pass_UV = in_UV;
                                                }
                                                ";

        private const string FRAGMENT_SHADER = @"
                                                #version 150 core
                                                in vec4 pass_Color;
                                                in vec2 pass_UV;

                                                out vec4 out_Color;
                                                
                                                uniform sampler2D inTex;
                                                uniform sampler2D inTex2;                                                

                                                void main(void) {
	                                                out_Color = pass_Color * texture(inTex, pass_UV);
                                                   
                                                }
                                                ";


        public GL_GUI_Context(Window window, float width, float height)
        {
            this.window = window;
            this.Width = width;
            this.Height = height;

            context = SDL.SDL_GL_CreateContext(window.Handle);
            SDL.SDL_GL_MakeCurrent(window.Handle, context);

            gl = new OpenGL();

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            shaderProgram = new ShaderProgram();
            shaderProgram.Create(gl, VERTEX_SHADER, FRAGMENT_SHADER, null); 
            shaderProgram.BindAttributeLocation(gl, 0, "in_Position");
            shaderProgram.BindAttributeLocation(gl, 1, "in_Color");
            shaderProgram.AssertValid(gl);

            gl.GenFramebuffersEXT(1, framebuffers);
            gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, framebuffers[0]);

            gl.GenTextures(1, framebufferTargets);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, framebufferTargets[0]);
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, window.DisplayWidth, window.DisplayHeight, 0, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.FramebufferTexture(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, framebufferTargets[0], 0);
            gl.DrawBuffers(1, new uint[] { OpenGL.GL_COLOR_ATTACHMENT0_EXT });

            if (gl.CheckFramebufferStatusEXT(OpenGL.GL_FRAMEBUFFER_EXT) != OpenGL.GL_FRAMEBUFFER_COMPLETE_EXT)
                throw new Exception("Frame buffer setup not complete");

             gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
             gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);


            ortho = new float[]{
                2.0f / Width, 0.0f, 0.0f, 0.0f,
                0.0f, -2.0f / Height, 0.0f, 0.0f,
                0.0f, 0.0f,-1.0f, 0.0f,
                -1.0f,1.0f, 0.0f, 1.0f,
            };
            /* setup global state */
            gl.Viewport(0, 0, window.DisplayWidth, window.DisplayHeight);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Disable(OpenGL.GL_CULL_FACE);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.Enable(OpenGL.GL_SCISSOR_TEST);

            shaderProgram.Bind(gl);
            shaderProgram.SetUniformMatrix4(gl, "projectionMatrix", ortho);

            vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);
        }


        public override GL_Texture CreateTexture(int W, int H, IntPtr Data)
        {
            return new GL_Texture(gl, W, H, Data);
        }


        public override void SetBuffer(NkVertex[] VertexBuffer, ushort[] IndexBuffer)
        {
            Verts = VertexBuffer;
            Inds = IndexBuffer;
        }

        public override void BeginRender()
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT | OpenGL.GL_SCISSOR_BIT);
        }

        public void glScissor2(int WindH, int X, int Y, int W, int H)
        {
            gl.Scissor(X, WindH - Y - H, W, H);
        }

        public void BeginBuffering()
        {
            gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, framebuffers[0]);
           // byte[] data = new byte[window.DisplayWidth * window.DisplayHeight * 4];
           // RenderTexture = new IntPtr();
           // gl.ReadPixels(0, 0, window.DisplayWidth, window.DisplayHeight, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_INT_8_8_8_8, data);
           // Marshal.Copy(data, 0, RenderTexture, data.Length);
        }


        public override void Render(NkHandle Userdata, GL_Texture Texture, NkRect ClipRect, uint Offset, uint Count)
        {
            float[] vert = new float[Count * 2];
            float[] uvs = new float[Count * 2];
            float[] colors = new float[Count * 4];

            for (int i = 0; i < Count; i++)
            {
                NkVertex V = Verts[Inds[Offset + i]];
                vert[i * 2] = V.Position.X;
                vert[i * 2 + 1] = V.Position.Y;

                uvs[i * 2] = V.UV.X;
                uvs[i * 2 + 1] = V.UV.Y;

                NkColorF colorf = V.GetColorF;
                colors[i * 4] = colorf.R;
                colors[i * 4 + 1] = colorf.G;
                colors[i * 4 + 2] = colorf.B;
                colors[i * 4 + 3] = colorf.A;
            }


            var vertexDataBuffer = new VertexBuffer();
            vertexDataBuffer.Create(gl);
            vertexDataBuffer.Bind(gl);
            vertexDataBuffer.SetData(gl, 0, vert, false, 2);

            var colourDataBuffer = new VertexBuffer();
            colourDataBuffer.Create(gl);
            colourDataBuffer.Bind(gl);
            colourDataBuffer.SetData(gl, 1, colors, false, 4);

            var uvDataBuffer = new VertexBuffer();
            uvDataBuffer.Create(gl);
            uvDataBuffer.Bind(gl);
            uvDataBuffer.SetData(gl, 2, uvs, false, 2);

            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, Texture.TextureName);
            glScissor2(window.DisplayHeight, (int)ClipRect.X, (int)ClipRect.Y, (int)ClipRect.W, (int)ClipRect.H);
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, (int)Count);

            vertexDataBuffer.Unbind(gl);
            colourDataBuffer.Unbind(gl);
            uvDataBuffer.Unbind(gl);
        }


        public void EndBuffering()
        {
         
        }

        public override void EndRender()
        {
            gl.Disable(OpenGL.GL_SCISSOR_TEST);

            gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);

            //vertexBufferArray.Unbind(gl);
            //shaderProgram.Unbind(gl);
        }

        public void DisplayRT()
        {
            float[] vert = new float[]
            {
                0f, 0f,
                0, window.DisplayHeight,
                window.DisplayWidth,  window.DisplayHeight,

                0f, 0f,
                window.DisplayWidth,  window.DisplayHeight,
                window.DisplayWidth, 0
            };

            float[] uvs = new float[]
            {
                0f, 1f,
                0f, 0f,
                1f, 0f,

                0f, 1f,
                1f, 0f,
                1f, 1f
            };

            float[] colors = new float[]
            {
                1f, 1f, 1f, 1f,
                1f, 1f, 1f, 1f,
                1f, 1f, 1f, 1f,

                1f, 1f, 1f, 1f,
                1f, 1f, 1f, 1f,
                1f, 1f, 1f, 1f,
            };

            var vertexDataBuffer = new VertexBuffer();
            vertexDataBuffer.Create(gl);
            vertexDataBuffer.Bind(gl);
            vertexDataBuffer.SetData(gl, 0, vert, false, 2);

            var colourDataBuffer = new VertexBuffer();
            colourDataBuffer.Create(gl);
            colourDataBuffer.Bind(gl);
            colourDataBuffer.SetData(gl, 1, colors, false, 4);

            var uvDataBuffer = new VertexBuffer();
            uvDataBuffer.Create(gl);
            uvDataBuffer.Bind(gl);
            uvDataBuffer.SetData(gl, 2, uvs, false, 2);

            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, framebufferTargets[0]);
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 12);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            vertexDataBuffer.Unbind(gl);
            colourDataBuffer.Unbind(gl);
            uvDataBuffer.Unbind(gl);
        }

        public void RenderFinal()
        {
            DisplayRT();
            SDL.SDL_GL_SwapWindow(window.Handle);
        }
    }
}
