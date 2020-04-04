public class Keypad
{
    private readonly byte[] keys;

    public Keypad()
    {
        keys = new byte[0xF+1];
    }

    public void Init()
    {
        for(int i=0; i<keys.Length; i++)
            keys[i] = 0x0;
    }

    public bool IsDown(byte k)
    {
        return keys[k]==0x1?true:false;
    }

    public byte IsDown()
    {
        for(byte i=0x0; i<0xF+1; i++)
            if(keys[i] == 0x1)
                return i;
        return 0xFF;
    }
}