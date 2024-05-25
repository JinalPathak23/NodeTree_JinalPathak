using NodeTree.Domain;

namespace NodeTree.Services
{
    public interface INodeService
    {
        /// <summary>
        /// Get all node 
        /// </summary>
        /// <returns></returns>
        Task<List<Nodes>> GetAllNodeAsync();

        /// <summary>
        /// Insert node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        Task InsertNodeAsync(Nodes node);

        /// <summary>
        /// Update node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        Task UpdateNodeAsync(Nodes node);

        /// <summary>
        /// Delete node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        Task DeleteNodeAndDescendantsAsync(Nodes node);

        /// <summary>
        /// Get node by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Nodes> GetNodeByIdAsync(int? id);
       
    }
}
