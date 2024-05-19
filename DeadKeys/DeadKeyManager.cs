using System.DirectoryServices;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Configuration;

namespace Avat.SingleLanguageEditor.DeadKeys;

internal class DeadKeyManager
{
    public DeadKeyManager(IConfiguration configuration)
    {
        _comboDefs = KeyComboDefConfig.Setup(configuration);
        _internalDeadKeyManager = new IdleDeadKeyManager(this);
    }

    public void Reset()
    {
        _internalDeadKeyManager = new IdleDeadKeyManager(this);
    }

    public bool Put(Keys inputKey)
    {
        if ((inputKey | Modifiers) == Modifiers) return false;
        var newDeadKeyManager = _internalDeadKeyManager.Process(inputKey);
        if (newDeadKeyManager == _internalDeadKeyManager) return false;
        _internalDeadKeyManager = newDeadKeyManager;
        return true;
    }

    public char? Get()
    {
        var result = _internalDeadKeyManager.OutputChar;
        if (result != null) Reset();
        return result;
    }

    private const Keys Modifiers = Keys.Shift | Keys.Control | Keys.Alt
        | Keys.ShiftKey | Keys.ControlKey | Keys.Menu;

    private IInternalDeadKeyManager _internalDeadKeyManager;
    private readonly IEnumerable<KeyComboDef> _comboDefs;

    private IInternalDeadKeyManager? Advance(IEnumerable<KeyComboDef> comboDefs, Keys inputKey)
    {
        var filterComboDefs = comboDefs
            .Where(d => d.FirstKey == inputKey)
            .Select(d => d.AfterFirstKey());
        if (!filterComboDefs.Any()) return null;

        var completeComboDef = filterComboDefs.FirstOrDefault(d => !d.AllKeys.Any());
        if (completeComboDef != null) return new CompletedDeadKeyManager(completeComboDef.OutputChar);

        return new InProgressDeadKeyManager(this, filterComboDefs);
    }


    private interface IInternalDeadKeyManager
    {
        char? OutputChar { get; }
        IInternalDeadKeyManager Process(Keys inputKey);
    }


    private class IdleDeadKeyManager: IInternalDeadKeyManager
    {
        public IdleDeadKeyManager(DeadKeyManager parent) 
            => _parent = parent;

        public char? OutputChar => null;

        public IInternalDeadKeyManager Process(Keys inputKey)
        {
            var candidateDeadKeyManager = _parent.Advance(_parent._comboDefs, inputKey);
            return candidateDeadKeyManager ?? this;
        }

        private readonly DeadKeyManager _parent; 
    }


    private class InProgressDeadKeyManager: IInternalDeadKeyManager
    {
        public InProgressDeadKeyManager(DeadKeyManager parent, IEnumerable<KeyComboDef> comboDefs)
            => (_parent, _comboDefs) = (parent, comboDefs.ToArray());

        public char? OutputChar => null;

        public IInternalDeadKeyManager Process(Keys inputKey)
        {
            var candidateDeadKeyManager = _parent.Advance(_comboDefs, inputKey);
            return candidateDeadKeyManager ?? new IdleDeadKeyManager(_parent);
        }

        private readonly DeadKeyManager _parent; 
        private readonly KeyComboDef[] _comboDefs;
    }


    private class CompletedDeadKeyManager: IInternalDeadKeyManager
    {
        public CompletedDeadKeyManager(char outputChar)
            => _outputChar = outputChar;

        public char? OutputChar => _outputChar;

        public IInternalDeadKeyManager Process(Keys inputKey)
            => throw new InvalidOperationException();

        private readonly char _outputChar;
    }
}