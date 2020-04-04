using System.Collections.Generic;

public class Chip8 
{
    Memory ram;
    private byte[] v;
    private ushort i;
    private byte dt, st;
    private ushort pc;
    private byte sp;
    private Stack<ushort> stack;
    private Keypad keypad;
    private GFXMemory vram;

    public Chip8()
    {
        ram = new Memory();
        v = new byte[16];
        stack = new Stack<ushort>();
        keypad = new Keypad();
        vram = new GFXMemory();
    }

    public Memory Ram { get => ram; set => ram = value; }
    public byte[] V { get => v; set => v = value; }
    public ushort I { get => i; set => i = value; }
    public byte Dt { get => dt; set => dt = value; }
    public byte St { get => st; set => st = value; }
    public ushort Pc { get => pc; set => pc = value; }
    public byte Sp { get => sp; set => sp = value; }
    public Stack<ushort> Stack { get => stack; set => stack = value; }
    public Keypad Keypad { get => keypad; set => keypad = value; }
    public GFXMemory Vram { get => vram; set => vram = value; }

    public void Init()
    {
        i = 0x0; dt = 0x0; st = 0x0; pc = 0x0; sp = 0x0;
        stack.Clear();

        for(int i=0; i<v.Length; i++)
            v[i] = 0x0;

        keypad.Init();
        ram.Init();
        vram.Init();
    }
}