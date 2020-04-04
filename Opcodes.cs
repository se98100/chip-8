using System;

public class Opcodes 
{
    private Random rand;
    private Chip8 chip;
    private ushort opcode;
    private readonly Action[] ops;
    private readonly Action[] subOps0;
    private readonly Action[] subOps8;
    private readonly Action[] subOpsE;
    private readonly Action[] subOpsF;

    public Opcodes(Chip8 chip)
    {
        rand = new Random();

        this.chip = chip;
        ops = new Action[0xF+1]
        {
            O0x0, O0x1, O0x2, O0x3,
            O0x4, O0x5, O0x6, O0x7,
            O0x8, O0x9, O0xA, O0xB,
            O0xC, O0xD, O0xE, O0xF
        };

        subOps0 = new Action[0xE+1];
        subOps0[0x0] = O0x00E0;
        subOps0[0xE] = O0x00EE;

        subOps8 = new Action[0xE+1];
        subOps8[0x0] = O0x8XY0;
        subOps8[0x1] = O0x8XY1;
        subOps8[0x2] = O0x8XY2;
        subOps8[0x3] = O0x8XY3;
        subOps8[0x4] = O0x8XY4;
        subOps8[0x5] = O0x8XY5;
        subOps8[0x6] = O0x8XY6;
        subOps8[0x7] = O0x8XY7;
        subOps8[0xE] = O0x8XYE;

        subOpsE = new Action[0xA+1];
        subOpsE[0x9] = O0xEX9E;
        subOpsE[0xA] = O0xEXA1;

        subOpsF = new Action[0x65+1];
        subOpsF[0x07] = O0xFX07;
        subOpsF[0x0A] = O0xFX0A;
        subOpsF[0x15] = O0xFX15;
        subOpsF[0x18] = O0xFX18;
        subOpsF[0x1E] = O0xFX1E;
        subOpsF[0x29] = O0xFX29;
        subOpsF[0x33] = O0xFX33;
        subOpsF[0x55] = O0xFX55;
        subOpsF[0x65] = O0xFX65;
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

    private void O0x4()
    {
        //Skip next instruction if Vx != kk.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        byte kk = (byte)(opcode & 0x00FF);
        if(vx != kk)
            chip.Pc += 4;
        else
            chip.Pc += 2;
    }

    private void O0x5()
    {
        //Skip next instruction if Vx = Vy.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        byte vy = chip.V[(opcode & 0x00F0) >> 4];
        if(vx == vy)
            chip.Pc += 4;
        else
            chip.Pc += 2;
    }

    private void O0x6()
    {
        //Set Vx = kk.
        chip.V[(opcode & 0x0F00) >> 8] = (byte)(opcode & 0x00FF);
        chip.Pc += 2;
    }

    private void O0x7()
    {
        //Set Vx = Vx + kk.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        chip.V[(opcode & 0x0F00) >> 8] = (byte)(vx + (opcode & 0x00FF));
        chip.Pc += 2;
    }

    private void O0x8()
    {
        subOps8[opcode & 0x000F]();
    }
    private void O0x8XY0()
    {
        //Set Vx = Vy.
        chip.V[(opcode & 0x0F00) >> 8] = chip.V[(opcode & 0x00F0) >> 4];
        chip.Pc += 2;
    }
    private void O0x8XY1()
    {
        //Set Vx = Vx OR Vy.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        byte vy = chip.V[(opcode & 0x00F0) >> 4];
        chip.V[(opcode & 0x0F00) >> 8] = (byte)(vx | vy);
        chip.Pc += 2;
    }
    private void O0x8XY2()
    {
        //Set Vx = Vx AND Vy.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        byte vy = chip.V[(opcode & 0x00F0) >> 4];
        chip.V[(opcode & 0x0F00) >> 8] = (byte)(vx & vy);
        chip.Pc += 2;
    }
    private void O0x8XY3()
    {
        //Set Vx = Vx XOR Vy.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        byte vy = chip.V[(opcode & 0x00F0) >> 4];
        chip.V[(opcode & 0x0F00) >> 8] = (byte)(vx ^ vy);
        chip.Pc += 2;
    }
    private void O0x8XY4()
    {
        //Set Vx = Vx + Vy, set VF = carry.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        byte vy = chip.V[(opcode & 0x00F0) >> 4];
        if(0xFF - vx < vy)
            chip.V[0xF] = 0x1;
        else
            chip.V[0xF] = 0x0;
        chip.V[(opcode & 0x0F00) >> 8] = (byte)(vx + vy);
        chip.Pc += 2;
    }
    private void O0x8XY5()
    {
        //Set Vx = Vx - Vy, set VF = NOT borrow.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        byte vy = chip.V[(opcode & 0x00F0) >> 4];
        if(vx >= vy)
            chip.V[0xF] = 0x1;
        else
            chip.V[0xF] = 0x0;
        chip.V[(opcode & 0x0F00) >> 8] = (byte)(vx - vy);
        chip.Pc += 2;
    }
    private void O0x8XY6()
    {
        //Set Vx = Vx SHR 1.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        if((vx & 0x01) == 0x1)
            chip.V[0xF] = 0x1;
        else
            chip.V[0xF] = 0x0;
        chip.V[(opcode & 0x0F00) >> 8] = (byte)(vx >> 1);
        chip.Pc += 2;
    }
    private void O0x8XY7()
    {
        //Set Vx = Vy - Vx, set VF = NOT borrow.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        byte vy = chip.V[(opcode & 0x00F0) >> 4];
        if(vy >= vx)
            chip.V[0xF] = 0x1;
        else
            chip.V[0xF] = 0x0;
        chip.V[(opcode & 0x0F00) >> 8] = (byte)(vy - vx);
        chip.Pc += 2;
    }
    private void O0x8XYE()
    {
        //Set Vx = Vx SHL 1.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        if((vx & 0x8) == 1)
            chip.V[0xF] = 0x1;
        else
            chip.V[0xF] = 0x0;
        chip.V[(opcode & 0x0F00) >> 8] = (byte)(vx << 1);
        chip.Pc += 2;
    }

    private void O0x9()
    {
        //Skip next instruction if Vx != Vy.
        byte vx = chip.V[(opcode & 0x0F00) >> 8];
        byte vy = chip.V[(opcode & 0x00F0) >> 4];
        if(vx != vy)
            chip.Pc += 4;
        else    
            chip.Pc += 2;
    }

    private void O0xA()
    {
        //Set I = nnn.
        chip.I = (byte)(opcode & 0x0FFF);
        chip.Pc += 2;
    }

    private void O0xB()
    {
        //Jump to location nnn + V0.
        chip.Pc = (byte)((opcode & 0x0FFF) + chip.V[0x0]);
    }

    private void O0xC()
    {
        //Set Vx = random byte AND kk.
        chip.V[(opcode & 0x0F00) >> 8] = (byte)((byte)rand.Next(256) & (opcode & 0x00FF));
        chip.Pc += 2;
    }

    private void O0xD()
    {
        //Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
        byte[] sprite = new byte[opcode & 0x000F];
        for(ushort i=chip.I; i<(ushort)((opcode & 0x000F) + chip.I); i++)
            sprite[i - chip.I] = chip.Ram.Read(i);
        chip.Vram.DrawSprite(sprite, (byte)(opcode & 0x000F), chip.V[(opcode & 0x0F00) >> 8], chip.V[(opcode & 0x00F0) >> 4]);
        if(chip.Vram.Collision)
            chip.V[0xF] = 0x1;
        else
            chip.V[0xF] = 0x0;
        chip.Pc += 2;
    }

    private void O0xE(){
        subOpsE[(opcode & 0x00F0) >> 4]();
    }
    private void O0xEX9E()
    {
        //Skip next instruction if key with the value of Vx is pressed.
        if(chip.Keypad.IsDown(chip.V[(opcode & 0x0F00) >> 8]))
            chip.Pc += 4;
        else
            chip.Pc += 2;
    }
    private void O0xEXA1()
    {
        //Skip next instruction if key with the value of Vx is not pressed.
        if(!chip.Keypad.IsDown(chip.V[(opcode & 0x0F00) >> 8]))
            chip.Pc += 4;
        else
            chip.Pc += 2;
    }

    private void O0xF()
    {
        subOpsF[opcode & 0x00FF]();
    }
    private void O0xFX07()
    {
        //Set Vx = delay timer value.
        chip.V[(opcode & 0x0F00) >> 8] = chip.Dt;
        chip.Pc += 2;
    }
    private void O0xFX0A()
    {
        //Wait for a key press, store the value of the key in Vx.
        byte k = chip.Keypad.IsDown();
        if(k != 0xFF)
        {
            chip.V[(opcode & 0x0F00) >> 8] = k;
            chip.Pc += 2;
        }
    }
    private void O0xFX15()
    {
        //Set delay timer = Vx.
        chip.Dt = chip.V[(opcode & 0x0F00) >> 8];
        chip.RegTimer.Start();
        chip.Pc += 2;
    }
    private void O0xFX18()
    {
        //Set sound timer = Vx.
        chip.St = chip.V[(opcode & 0x0F00) >> 8];
        chip.RegTimer.Start();
        chip.Pc += 2;
    }
    private void O0xFX1E()
    {
        //Set I = I + Vx.
        chip.I = (ushort)(chip.I + chip.V[(opcode & 0x0F00) >> 8]);
        chip.Pc += 2;
    }
    private void O0xFX29()
    {
        //Set I = location of sprite for digit Vx.
        chip.I = (ushort)(chip.V[(opcode & 0x0F00) >> 8] * 0x5 + 0x1);
        chip.Pc += 2;
    }
    private void O0xFX33()
    {
        //Store BCD representation of Vx in memory locations I, I+1, and I+2.
        byte n = chip.V[(opcode & 0x0F00) >> 8];
        byte hundreds = (byte)(n / 100);
        byte tens = (byte)((n - hundreds * 100) / 10);
        byte units = (byte)(n - (tens * 10 + hundreds * 100));
        chip.Ram.Write(hundreds, chip.I);
        chip.Ram.Write(tens, (byte)(chip.I + 0x1));
        chip.Ram.Write(units, (byte)(chip.I + 0x2));
        chip.Pc += 2; 
    }
    private void O0xFX55()
    {
        //Store registers V0 through Vx in memory starting at location I.
        for(byte i=0x0; i<=(byte)((opcode & 0x0F00) >> 8); i++)
            chip.Ram.Write(chip.V[i], (byte)(chip.I + i));
        chip.Pc += 2;
    }
    private void O0xFX65()
    {
        //Read registers V0 through Vx from memory starting at location I.
        for(byte i=0x0; i<=(byte)((opcode & 0x0F00) >> 8); i++)
            chip.V[i] = chip.Ram.Read((byte)(chip.I + i));
        chip.Pc += 2;
    }
}