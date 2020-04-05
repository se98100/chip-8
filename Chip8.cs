using System.Collections.Generic;
using System.IO;
using System.Timers;
using System;

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
    private Timer regTimer;
    private Opcodes opcodes;

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
    public Timer RegTimer { get => regTimer; set => regTimer = value; }

    public Chip8(GFXMemory vram)
    {
        ram = new Memory();
        v = new byte[16];
        stack = new Stack<ushort>();
        keypad = new Keypad();
        this.vram = vram;
        regTimer = new Timer(16.6666);
        regTimer.Elapsed += OnRegTimer;
        regTimer.AutoReset = true;
        opcodes = new Opcodes(this);
    }

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

    public void Load(string file)
    {
        byte[] program = File.ReadAllBytes(file);
        for(int i=0; i<program.Length; i++)
            ram.Write(program[i], (ushort)(i + 0x200));
        
        pc = 0x200;
    }

    public void Cycle()
    {
        ushort h = ram.Read(pc);
        byte l = ram.Read((ushort)(pc + 1));
        ushort opcode = (ushort)((h << 8) | l);
        opcodes.Exec(opcode);
    }

    private void OnRegTimer(Object o, ElapsedEventArgs e)
    {
        if(st > 0) st--;
        if(dt > 0) dt--;
        if(st == 0 && dt == 0) regTimer.Stop();
    }
}