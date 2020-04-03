
public class Chip8 {
    Memory ram;
    private byte[] v;
    private ushort i;
    private byte dt;
    private byte st; 
    private ushort pc;
    private byte sp;
    private ushort[] stack;

    public Chip8()
    {
        ram = new Memory();
        v = new byte[16];
        stack = new ushort[16];
    }
}