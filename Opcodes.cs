using System;

public class Opcodes 
{
    private Chip8 chip;
    private ushort opcode;
    private readonly Action[] ops;
    private readonly Action[] subOps0;

    public Opcodes(Chip8 chip)
    {
        this.chip = chip;
        ops = new Action[4]{O0x0, O0x1, O0x2, O0x3};

        subOps0 = new Action[15];
        subOps0[0x0] = O0x00E0;
        subOps0[0xE] = O0x00EE;
    }

    public void Exec(ushort opcode)
    {
        this.opcode = opcode;
        ops[(opcode & 0xF000) >> 12]();
    }

    private void O0x0()
    {
        subOps0[opcode & 0x000F]();
    }
    private void O0x00E0()
    {
        //Clear the display.
        chip.Vram.Init();
        chip.Pc += 2;
    }
    private void O0x00EE()
    {
        //Return from a subroutine.
        chip.Pc = chip.Stack.Pop();
        chip.Sp--;
        chip.Pc += 2;
    }

    private void O0x1()
    {
        //Jump to location nnn.
        chip.Pc = (ushort)(opcode & 0x0FFF);
    }

    private void O0x2()
    {
        //Call subroutine at nnn.
        chip.Sp++;
        chip.Stack.Push(chip.Pc);
        chip.Pc = (ushort)(opcode & 0x0FFF);
    }

    private void O0x3()
    {
        //Skip next instruction if Vx = kk.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        byte kk = (byte)(opcode & 0x00FF);
        if(vx == kk)
            chip.Pc += 4;
        else
            chip.Pc += 2;
    }
}