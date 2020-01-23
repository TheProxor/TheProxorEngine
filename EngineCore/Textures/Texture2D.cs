using System;
using System.Collections.Generic;
using System.Text;

using SharpGL;

namespace EngineCore.Textures
{
    public class Texture2D
    {
    }

    internal class GL_Texture2D : Texture2D
    {
        private uint[] glTextureArray = new uint[1] { 0 };

        private OpenGL gl;

        public GL_Texture2D(OpenGL gl, int W, int H, IntPtr Data)
        {
            this.gl = gl;
            gl.GenTextures(1, glTextureArray);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, TextureName);
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA, W, H, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, Data);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
        }

        ~GL_Texture2D()
        {
            gl.DeleteTextures(glTextureArray.Length, glTextureArray);
            glTextureArray[0] = 0;
        }

        public uint TextureName
        {
            get { return glTextureArray[0]; }
        }
    }

    internal class DX_Texture2D : Texture2D
    {

    }

    internal class VK_Texture2D : Texture2D
    {

    }
}
