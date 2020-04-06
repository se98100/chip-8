using System;

namespace chip_8
{
    class Program
    {
        static void Main(string[] args)
        {
            GFXMemory vram = new GFXMemory();
            
            Chip8 chip = new Chip8(vram);
            chip.Init();
            chip.Load(@"C:\Users\Sergio\Downloads\Blinky.ch8");

            Render win = new Render(1200, 600, "Test", vram, chip);
            win.Run(200);
        }
    }
}
