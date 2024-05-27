
// MIT License

// Copyright (c) 2024 Andrei Vatasescu

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Avat.SingleLanguageEditor.Utils;

namespace Avat.SingleLanguageEditor.Forms;

internal class TranslationFileView
{
    public TranslationFileView(ITranslationNode translationRoot, IXLiffHandler xliffHandler)
    {
        _xliffHandler = xliffHandler;
        _treeRoot = CreateTreeNode(translationRoot.LocalName, PrepareTreeNode(translationRoot));
    }

    public TreeNode TreeRoot => _treeRoot;
    
    public IEnumerable<ITranslationUnit> TranslationUnits 
        => _translationData.Select(_xliffHandler.GetTranslationUnit);

    public IEnumerable<ListViewItem> CreateListItems(TreeNode treeNode)
    {
        var itemData = new string[4];

        foreach(var subnode in treeNode.Nodes.OfType<TreeNode>())
        {
            itemData[0] = subnode.Text;
            itemData[1] = $"{TranslationStates.NotAvailable}";
            itemData[2] = string.Empty;
            itemData[3] = string.Empty;
            yield return new ListViewItem(itemData);
        }

        if (treeNode.Tag != null) 
        {
            var listItemContent = (IEnumerable<ListItemContent>) treeNode.Tag;
            foreach(var item in listItemContent)
            {
                var unit = _xliffHandler.GetTranslationUnit(item.ListItemData.Value);
                itemData[0] = item.Name;
                itemData[1] = $"{unit.State}";
                itemData[2] = unit.SourceText;
                itemData[3] = unit.TargetText;
                yield return new ListViewItem(itemData) { 
                    Tag = item.ListItemData
                };
            }
        }
    }

    public IEnumerable<ITranslationUnit> GetTranslationUnits(TreeNode treeNode)
        =>  GetTranslationUnits(GetFirstTranslationNode(treeNode));
    
    private LinkedListNode<object>? GetFirstTranslationNode(TreeNode node)
    {
        if (node.Tag is IEnumerable<ListItemContent> items)
        { 
            var result = items.Select(i => i.ListItemData).FirstOrDefault();
            if (result != null) return result;
        }

        foreach(var subnode in node.Nodes.OfType<TreeNode>())
        {
            var result = GetFirstTranslationNode(subnode);
            if (result != null) return result;
        }
        return null;
    }

    public IEnumerable<ITranslationUnit> GetTranslationUnits(ListViewItem item) 
        => GetTranslationUnits((LinkedListNode<object>) item.Tag);

    private IEnumerable<ITranslationUnit> GetTranslationUnits(LinkedListNode<object>? listNode) 
        => new RollingTranslationList(_translationData.First!, listNode, _translationData.Last!)
            .Select(_xliffHandler.GetTranslationUnit);

    public ITranslationUnit? GetTranslationUnit(ListViewItem? item) 
        => item != null && item.Tag != null ? GetTranslationUnit(item.Tag) : null;

    private static TreeNode CreateTreeNode(string name, TreeNodeContent content) 
        => new(name, content.Subnodes.ToArray()) { Tag = content.TreeNodeData };

    private readonly TreeNode _treeRoot;
    private readonly IXLiffHandler _xliffHandler;
    private readonly LinkedList<object> _translationData = new();

    private TreeNodeContent PrepareTreeNode(ITranslationNode node)
    {
        var subnodes = node
            .Subnodes
            .Where(n => n.Data == null)
            .Select(n => CreateTreeNode(n.LocalName, PrepareTreeNode(n)));   

        var treeNodeData = node
            .Subnodes
            .Where(n => n.Data != null)
            .Select(n => new ListItemContent(n.LocalName, _translationData.AddLast(n.Data!)))
            .ToArray();

        return new TreeNodeContent(treeNodeData, subnodes);     
    }

    private ITranslationUnit?  GetTranslationUnit(object listItemData)
        => listItemData is LinkedListNode<object> node ? _xliffHandler.GetTranslationUnit(node.Value) 
            : throw new Exception($"{listItemData}"); 
 
    private class TreeNodeContent
    {
        public TreeNodeContent(IEnumerable<ListItemContent> treeNodeData, IEnumerable<TreeNode> subnodes)
            => (TreeNodeData, Subnodes) = (treeNodeData, subnodes);

        public readonly IEnumerable<object> TreeNodeData;
        public readonly IEnumerable<TreeNode> Subnodes;
    }

    private class ListItemContent
    {
        public ListItemContent(string name, LinkedListNode<object> listItemData)
            => (Name, ListItemData) = (name, listItemData);

        public readonly string Name;
        public readonly LinkedListNode<object> ListItemData;
    }
}