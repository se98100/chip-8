public class Keypad
{
    private readonly byte[] keys;

    public Keypad()
    {
        keys = new byte[16];
    }

    public void Init()
    {
        for(int i=0; i<keys.Length; i++)
            keys[i] = 0x0;
    }
}