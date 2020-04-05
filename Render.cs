using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL4;

public class Render : GameWindow
{
    private GFXMemory vram;
    private Chip8 chip;
    private int vbo;
    private int vao;
    private int ebo;
    private Shader shader;
    private double[] vertices;
    private uint[] indices = 
    {
        0, 1, 66, 65
    };

    public Render(int width, int height, string title, GFXMemory vram, Chip8 chip) : base(width, height, GraphicsMode.Default, title){
        this.vram = vram;
        this.chip = chip;
        vertices = new double[24576];
        InitVertices();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        chip.Cycle();
        KeyboardState input = Keyboard.GetState();
        HandleKey(input, Key.Keypad0, 0x0);
        HandleKey(input, Key.Keypad1, 0x1);
        HandleKey(input, Key.Keypad2, 0x2);
        HandleKey(input, Key.Keypad3, 0x3);
        HandleKey(input, Key.Keypad4, 0x4);
        HandleKey(input, Key.Keypad5, 0x5);
        HandleKey(input, Key.Keypad6, 0x6);
        HandleKey(input, Key.Keypad7, 0x7);
        HandleKey(input, Key.Keypad8, 0x8);
        HandleKey(input, Key.Keypad9, 0x9);
        base.OnUpdateFrame(e);
    }

    private void HandleKey(KeyboardState input, Key k, byte v)
    {
        if(input.IsKeyDown(k))
            chip.Keypad.KeyDown(v);
        if(input.IsKeyUp(k))
            chip.Keypad.KeyUp(v);
    }

    protected override void OnLoad(System.EventArgs e)
    {
        shader = new Shader("shader.vert", "shader.frag");
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        vbo = GL.GenBuffer();
        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(double), vertices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Double, false, sizeof(double) * 3, 0);

        ebo = GL.GenBuffer();
        GL.EnableVertexAttribArray(0);

        base.OnLoad(e);
    }

    protected override void OnUnload(System.EventArgs e)
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(vbo);
        shader.Dispose();
        base.OnUnload(e);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        indices = new uint[vram.PixelsOn * 4];
        LoadIndices();

        GL.Clear(ClearBufferMask.ColorBufferBit);
        if(vram.PixelsOn > 0)
        {
            shader.Use();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);
            GL.DrawElements(BeginMode.Quads, vram.PixelsOn * 4, DrawElementsType.UnsignedInt, 0);
        }

        Context.SwapBuffers();
        base.OnRenderFrame(e);
    }

    protected override void OnResize(System.EventArgs e)
    {
        GL.Viewport(0, 0, Width, Height);
        base.OnResize(e);
    }

    private void InitVertices()
    {
        int c=0; double x = 0.03125, y = 0.0625;
        for(int i=0;i<32;i++)
        {
            for(int j=0;j<64;j++)
            {
                vertices[c++] = x*j -1.0; vertices[c++] = -y*i +1.0; vertices[c++] = 0;
                if(j == 63)
                {
                    vertices[c++] = x*(j+1) -1.0; vertices[c++] = -y*i +1.0; vertices[c++] = 0;
                }
                if(i == 31)
                {
                    vertices[c++] = x*j -1; vertices[c++] = -y*(i+1) +1; vertices[c++] = 0;
                    if(j == 63)
                        vertices[c++] = x*(j+1) -1; vertices[c++] = -y*(i+1) +1; vertices[c++] = 0;
                }
            }
        }
    }

    private void LoadIndices()
    {
        int c = 0;
        for(int i=0; i<32; i++)
        {
            for(int j=0; j<64; j++)
            {
                if(vram.Dump[i, j] == 0x1)
                {
                    indices[c++] = (uint)(j + 65 * i);
                    indices[c++] = (uint)(j + 65 * i + 1);
                    indices[c++] = (uint)(j + 65 * i + 65 + 1);
                    indices[c++] = (uint)(j + 65 * i + 65);
                }
            }
        }
    }
}