/// <summary>
/// RoutePlanner.cs
/// </summary>
namespace Networking
{
    using System.Collections.Generic;

    /// <summary>
    /// Route planner will be used to create generic routing methods. These methods should be able to be 
    /// called anywhere, given generic nodeModel input for both start and end points. 
    /// 
    /// Stategy: Use best connection type available, search each possible path looking for complete paths, 
    /// find least congested path, route. :) 
    /// </summary>
    public static class RoutePlanner
    {

        /// <summary>
        /// Plans the path.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="dst">The DST.</param>
        /// <returns>List&lt;NodeModel&gt;.</returns>
        public static List<NodeModel> PlanPath(NodeModel src, NodeModel dst)
        {
            var destinationFound = false;
            var pathList = new List<List<NodeModel>>();
            var currentPath = new List<NodeModel>();
            FindPaths(src, dst, ref pathList, currentPath, ref destinationFound);

            if (pathList.Count == 0)
            {
                return null; 
            }

            var bestPath = ChooseBestPath(pathList); 

            return bestPath;
        }

        /// <summary>
        /// This method shall compute all possible paths possible in the graph and return a list of all routes 
        /// </summary>
        /// <param name="src">Source node to start the search from</param>
        /// <param name="dst">Destination node to end the search at</param>
        /// <returns>List of the a list of paths</returns>
        private static List<NodeModel> FindPaths(NodeModel src, NodeModel dst, ref List<List<NodeModel>> pathList, List<NodeModel> currentPath, ref bool dstFound)
        {
            //add the current node to the currentPath list 
            currentPath.Add(src);

            //check if the destination has been found
            if (src.Equals(dst))
            {
                var list = new List<NodeModel>(currentPath);
                pathList.Add(list);
                dstFound = true;
                currentPath.Remove(src);
                return currentPath;
            }
            else
            {
                List<NodeModel> path = null;
                var connectionCount = 0; 

                //check if we've made it to a tower, then we will automatically route to the cmd centre 
                if (src.GetType() == typeof(TowerModel))
                {
                    foreach (var connection in src.connections)
                    {
                        if (connection.GetType() == typeof(CommandCenterModel))
                        {
                            FindPaths(connection, dst, ref pathList, currentPath, ref dstFound);

                            if (dstFound == true)
                            {
                                currentPath.Remove(src);
                            }

                            return currentPath; 
                        }
                    }
                }
                else
                {

                    connectionCount = src.connections.Count;

                    //for every node connection
                    foreach (var connection in src.connections.ToArray())
                    {
                      
                        var a = connection.NodeId;
                        var b = src.NodeId;
                        //travel to the node link 
                        if (!currentPath.Contains(connection))
                        {
                            path = FindPaths(connection, dst, ref pathList, currentPath, ref dstFound);
                        }

                        if (null == path && connectionCount == 1 && !IsStartNode(currentPath, src))
                        {
                            //determine previous node
                            var listCount = currentPath.Count;
                            var previousNode = currentPath[listCount - 1]; 
                            previousNode.connections.Remove(src);
                                                                                                      
                            currentPath.Remove(src);

                            return null;
                        }

                        connectionCount--;
                        
                    }

                    if (null == path && IsStartNode(currentPath, src))
                    {
                        currentPath = new List<NodeModel>();
                        dstFound = false;
                    }
                    else if (dstFound == true)
                    {
                        currentPath.Remove(src);
                    }

                }
            }

            return currentPath;
        }

        /// <summary>
        /// Determines whether [is start node] [the specified current path].
        /// </summary>
        /// <param name="currentPath">The current path.</param>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if [is start node] [the specified current path]; otherwise, <c>false</c>.</returns>
        private static bool IsStartNode(List<NodeModel> currentPath, NodeModel target)
        {
            if (target.Equals(currentPath[0]))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Chooses the best path.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns>List&lt;NodeModel&gt;.</returns>
        private static List<NodeModel> ChooseBestPath(List<List<NodeModel>> paths)
        {
            var currentHops = 0;
            var minHops = int.MaxValue;
            var index = 0;
            var saveIndex = 0;
            //loops through the main path in search for the best path count
            foreach (var path in paths)
            {
                

                currentHops = path.Count;
                //if the current path is smaller than the current min path, update the min path
                if (currentHops < minHops)
                {
                    minHops = currentHops;
                    saveIndex = index;
                }

                currentHops = 0;
                index++;
            }
            //return shortest path
            return paths[saveIndex];
        }
    }
}
