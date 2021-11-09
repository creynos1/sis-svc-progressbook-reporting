using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewsExagoDependencies
{
    public class FolderNameParser
    {
        private class TreeNode
        {
            public TreeNode Parent { get; set; }
            public List<TreeNode> Children { get; set; } = new List<TreeNode>();
            public ReportEntity Data { get; set; }
            public string Path { get; set; }
        }

        public static Dictionary<Guid, string> GetPaths(IEnumerable<ReportEntity> folders)
        {
            var paths = new Dictionary<Guid, string>();
            paths.Add(Guid.Empty, "");


            //Depth first search
            foreach(var root in BuildTree(folders))
            {
                root.Path = root.Data.Name;
                var stack = new Stack<TreeNode>();
                StackAddChildrenWithPath(root.Children, root.Path, stack);
                paths.Add(root.Data.ReportEntityId, root.Path);

                while(stack.Count > 0)
                {
                    var item = stack.Pop();
                    StackAddChildrenWithPath(item.Children, item.Path, stack);
                    paths.Add(item.Data.ReportEntityId, item.Path);
                }
            }

            return paths;
        }

        private static List<TreeNode> BuildTree(IEnumerable<ReportEntity> folders)
        {
            var roots = new List<TreeNode>();
            var currentNodes = new Dictionary<Guid, TreeNode>();

            foreach(var folder in folders)
            {
                TreeNode currentNode;
                if(currentNodes.TryGetValue(folder.ReportEntityId, out currentNode))
                {
                    currentNode.Data = folder;
                }
                else
                {
                    currentNode = new TreeNode() { Data = folder };
                    currentNodes.Add(folder.ReportEntityId, currentNode);
                }

                if(folder.ParentId == null)
                {
                    roots.Add(currentNode);
                }
                else
                {
                    var parentId = folder.ParentId ?? Guid.Empty;
                    TreeNode parentNode;
                    if(!currentNodes.TryGetValue(parentId, out parentNode))
                    {
                        parentNode = new TreeNode();
                        currentNodes.Add(parentId, parentNode);
                    }
                    currentNode.Parent = parentNode;
                    parentNode.Children.Add(currentNode);
                }
            }

            return roots;
        }

        private static Stack<TreeNode> StackAddChildrenWithPath(IEnumerable<TreeNode> items, string path, Stack<TreeNode> stack)
        {
            foreach(var item in items)
            {
                item.Path = $"{path}/{item.Data.Name}";
                stack.Push(item);
            }
            return stack;
        }
    }


}
