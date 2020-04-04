public class GFXMemory
{
    private const int width = 64;
    private const int height = 32;
    private byte[,] mem;
    private bool collision;

    public bool Collision { get => collision; set => collision = value; }

    public GFXMemory()
    {
        mem = new byte[height, width];
    }

    public void Init()
    {
        collision = false;
        for(int i=0; i<height; i++)
            for(int j=0; j<width; j++)
                mem[i, j] = 0x0;
    }

    public void DrawSprite(byte[] sprite, byte n, byte x, byte y)
    {
        collision = false;
        byte yStart = y;
        for(byte i=0; i<n; i++)
        {
            for(byte b=0; b<8; b++)
            {
                if(mem[x, y] == 0x1 && sprite[(sprite[i] >> b) & 0x01] == 0x1)
                    collision = true;
                mem[x, y] ^= (byte)((sprite[i] >> b) & 0x01);
                if(y == width-1)
                    y = 0;
                else
                    y++;
            }

            if(x == height-1)
                x = 0;
            else
                x++;
            y = yStart;
        }
    }
}