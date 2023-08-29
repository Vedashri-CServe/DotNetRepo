using DocumentFormat.OpenXml.EMMA;
using System.Linq;
using TMS.Entity;

namespace TMS.Helper.UtilityHelper
{
    
    public static class TreeBuilder
    {  
      
        public static IEnumerable<Tree<T>> GenerateTree<T, K>(
        this IEnumerable<T> flatObjects,
        Func<T, K> id_selector,
        Func<T, K> parent_id_selector,
        K root_id = default(K)
        ) where T : class
        {

            //foreach (var c in flatObjects.Where(c => EqualityComparer<K>.Default.Equals(parent_id_selector(c), root_id)))
            var res = flatObjects.Where(c => EqualityComparer<K>.Default.Equals(parent_id_selector(c), root_id)).Select(item => new Tree<T>
            {
                Parent = item,
                Children = GenerateTree(flatObjects, id_selector, parent_id_selector, id_selector(item))
            }).ToList();
            return res;
        }

        public static IEnumerable<Tree<T>> Descendants<T>(Tree<T> root) where T : class
        {
            var nodes = new Stack<Tree<T>>(new[] { root });
            while (nodes.Any())
            {
                Tree<T> node = nodes.Pop();
                yield return node;
                foreach (var n in node.Children) nodes.Push(n);
            }
        }

        public static IEnumerable<T> Flatten<T>(IEnumerable<Tree<T>> e) where T : class
        {
            return e.SelectMany(c => Flatten(c.Children)).Concat(e.Select(i => i.Parent).OfType<T>());
        }
       
        
    }
}