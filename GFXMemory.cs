public class GFXMemory
{
    private const int width = 64;
    private const int height = 32;
    private byte[,] mem;

    public GFXMemory()
    {
        mem = new byte[height, width];
    }

    public void Init()
    {
        for(int i=0; i<height; i++)
            for(int j=0; j<width; j++)
                mem[i, j] = 0x0;
    }
}