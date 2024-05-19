namespace MLE.DeadKeys;

internal class KeyComboDef 
{
    public KeyComboDef(IEnumerable<Keys> allKeys, char outputChar)
        => (AllKeys, OutputChar) = (allKeys, outputChar);

    public readonly IEnumerable<Keys> AllKeys;
    public readonly char OutputChar;

    public Keys? FirstKey => AllKeys.FirstOrDefault();

    public KeyComboDef AfterFirstKey() 
        => new (AllKeys.Skip(1), OutputChar);
}
