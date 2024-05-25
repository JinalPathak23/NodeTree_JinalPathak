using Microsoft.EntityFrameworkCore;
using NodeTree.Data;
using NodeTree.Domain;

namespace NodeTree.Services
{
    public class NodeService : INodeService
    {
        #region Fields

        private readonly NodeDbContext _dbContext;

        #endregion

        #region  ctor

        public NodeService(NodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all node 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<Nodes>> GetAllNodeAsync()
        {
            var nodes = await _dbContext.Nodes.ToListAsync(); // Await the asynchronous database call
            return nodes;
        }

        /// <summary>
        /// Insert node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual async Task InsertNodeAsync(Nodes node)
        {
            _dbContext.Nodes.Add(node);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Update node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual async Task UpdateNodeAsync(Nodes node)
        {
            _dbContext.Nodes.Update(node);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Delete node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual  async Task DeleteNodeAndDescendantsAsync(Nodes node)
        {

            var childNodes = _dbContext.Nodes.Where(n => n.ParentNodeId == node.NodeId).ToList();

            // Delete each child node and its descendants recursively
            foreach (var childNode in childNodes)
            {
                DeleteNodeAndDescendantsAsync(childNode);
            }
            _dbContext.Nodes.Remove(node);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Get node by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<Nodes> GetNodeByIdAsync(int? id)
        {
            var query = from Cmm in _dbContext.Nodes
                        where Cmm.NodeId == id
                        select Cmm;
            return await query.FirstOrDefaultAsync();
        }
      
        #endregion
    }
}
