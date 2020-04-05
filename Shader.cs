using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;

public class Shader : System.IDisposable
{
    private int handle;
    private bool disposed = false;

    public int Handle { get => handle; set => handle = value; }

    public Shader(string vertex, string fragment)
    {
        int vShader, fShader;
        string vSource, fSource;

        using(StreamReader reader = new StreamReader(vertex, Encoding.UTF8))
        {
            vSource = reader.ReadToEnd();
        }
        using(StreamReader reader = new StreamReader(fragment, Encoding.UTF8))
        {
            fSource = reader.ReadToEnd();
        }

        vShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vShader, vSource);
        fShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fShader, fSource);
        GL.CompileShader(vShader);
        string log = GL.GetShaderInfoLog(vShader);
        if(log != System.String.Empty)
            System.Console.WriteLine(log);
        GL.CompileShader(fShader);
        log = GL.GetShaderInfoLog(fShader);
        if(log != System.String.Empty)
            System.Console.WriteLine(log);

        handle = GL.CreateProgram();
        GL.AttachShader(handle, vShader);
        GL.AttachShader(handle, fShader);
        GL.LinkProgram(handle);

        GL.DetachShader(handle, vShader);
        GL.DetachShader(handle, fShader);
        GL.DeleteShader(vShader);
        GL.DeleteShader(fShader);
    }

    public void Use()
    {
        GL.UseProgram(handle);
    }

    public void Dispose()
    {
        if(!disposed)
        {
            GL.DeleteProgram(handle);
            disposed = true;
        }
    }
}