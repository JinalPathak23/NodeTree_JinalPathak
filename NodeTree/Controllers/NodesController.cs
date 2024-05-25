using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NodeTree.Data;
using NodeTree.Domain;
using NodeTree.Models;
using NodeTree.Services;

namespace NodeTree.Controllers
{
    public class NodesController : Controller
    {
        #region Fields

        private readonly NodeDbContext _dbContext;
        private readonly INodeService _nodeService;

        #endregion

        #region  ctor

        public NodesController(NodeDbContext dbContext, INodeService nodeService)
        {
            _dbContext = dbContext;
            _nodeService = nodeService;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<ActionResult> List()
        {
            var nodes = await _nodeService.GetAllNodeAsync(); 
            var model = new List<NodeViewModel>();

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    string parentNodeName = string.Empty;
                    if (node.ParentNodeId != 0) // Check if it's not the root node
                    {
                        var parentNode = _dbContext.Nodes.FirstOrDefault(n => n.NodeId == node.ParentNodeId);
                        if (parentNode != null)
                        {
                            parentNodeName = parentNode.NodeName;
                        }
                    }
                    else
                    {
                        parentNodeName = "Its Parent Node"; // Set default value
                    }

                    var data = new NodeViewModel
                    {
                        NodeId = node.NodeId,
                        NodeName = node.NodeName,
                        ParentNodeName = parentNodeName,
                        StartDate = node.StartDate,
                        IsActive = node.IsActive,
                    };
                    model.Add(data); // Add NodeViewModel to the list
                }
            }
            return View(model);
        }

    
        [HttpGet]
        public ActionResult Create()
        {
            var nodeNames = _dbContext.Nodes.Where(node => node.IsActive).Select(node => new SelectListItem
            {
                Value = node.NodeId.ToString(), 
                Text = node.NodeName
            }).ToList();
            ViewBag.NodeNames = nodeNames;
            return View();
        }

        [HttpPost]
        public ActionResult Create(NodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var node = new Nodes
                {
                    NodeId = model.NodeId,
                    NodeName = model.NodeName,
                    ParentNodeId = model.ParentNodeId ?? 0,
                    IsActive = model.IsActive,
                    StartDate = DateTime.UtcNow,

                };               
               _nodeService.InsertNodeAsync(node);
                TempData["SuccessMessage"] = "Node added successfully.";
                return RedirectToAction("List");
            }
            var nodeNames = _dbContext.Nodes.Select(node => new SelectListItem
            {
                Value = node.NodeId.ToString(),
                Text = node.NodeName
            }).ToList();
            ViewBag.NodeNames = nodeNames;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var nodes = await _nodeService.GetNodeByIdAsync(id);
            if (nodes == null)
            {
                return RedirectToAction("List");
            }
            var model = new NodeViewModel
            {
                NodeId = nodes.NodeId,
                NodeName = nodes.NodeName,
                ParentNodeId = nodes.ParentNodeId,
                IsActive = nodes.IsActive,
                StartDate = nodes.StartDate,
            };
            var nodeNames = _dbContext.Nodes.Where(node => node.IsActive).Select(node => new SelectListItem
            {
                Value = node.NodeId.ToString(),
                Text = node.NodeName
            }).ToList();
            ViewBag.NodeNames = nodeNames;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nodeNames = _dbContext.Nodes.Where(node => node.IsActive).Select(node => new SelectListItem
                {
                    Value = node.NodeId.ToString(), 
                    Text = node.NodeName
                }).ToList();
                ViewBag.NodeNames = nodeNames;

                var node = await _nodeService.GetNodeByIdAsync(model.NodeId);
                if (node == null)
                {
                    return RedirectToAction("List");
                }
                node.NodeName = model.NodeName;
                node.ParentNodeId = model.ParentNodeId ?? 0;
                node.IsActive = model.IsActive;
                node.StartDate = DateTime.UtcNow;
                if (!model.IsActive)
                {
                    var childNodes = _dbContext.Nodes.Where(n => n.ParentNodeId == model.NodeId);
                    foreach (var childNode in childNodes)
                    {
                        childNode.IsActive = false;
                    }
                }
                _nodeService.UpdateNodeAsync(node);
                TempData["SuccessMessage"] = "Node updated successfully.";
                return RedirectToAction("List");
            }
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var node = await _nodeService.GetNodeByIdAsync(id);
            if (node == null)
            {
                return RedirectToAction("List");
            }

            // Delete the node and all its descendants recursively
            _nodeService.DeleteNodeAndDescendantsAsync(node);

            TempData["SuccessMessage"] = "Node and all its descendants deleted successfully.";
            return RedirectToAction("List");
        }
        public async Task<IActionResult> Details(int? id)
        {
            var nodes = await _nodeService.GetNodeByIdAsync(id);
            if (nodes == null)
            {
                return RedirectToAction("List");
            }
            var parentNodeName = _dbContext.Nodes.Where(n => n.NodeId == nodes.ParentNodeId).Select(n => n.NodeName).FirstOrDefault();
           
            ViewBag.ParentNodeName = parentNodeName;

            ViewBag.StartDate = nodes.StartDate.ToShortDateString();
            return View(nodes);

        }
       /// <summary>
       /// Tree view of nodes
       /// </summary>
       /// <returns></returns>
        public async Task<ActionResult> TreeView()
        {
            //call store procedure
            var activeNodes = await _dbContext.Nodes.FromSqlRaw("EXECUTE GetActiveNodes").ToListAsync();

            var treeNodes = activeNodes.Select(node => new NodeViewModel
            {
                NodeId = node.NodeId,
                NodeName = node.NodeName,
                ParentNodeId = node.ParentNodeId,
                IsActive = node.IsActive,
                StartDate = node.StartDate,
                Children = new List<NodeViewModel>()
            }).ToList();

            foreach (var node in treeNodes)
            {
                if (node.ParentNodeId.HasValue)
                {
                    var parentNode = treeNodes.FirstOrDefault(n => n.NodeId == node.ParentNodeId);
                    if (parentNode != null)
                    {
                        parentNode.Children ??= new List<NodeViewModel>();
                        parentNode.Children.Add(node);
                    }
                }
            }
            var topLevelNodes = treeNodes.Where(node => !activeNodes.Any(n => n.NodeId == node.ParentNodeId)).ToList();

            return View(topLevelNodes);
        }
        #endregion

    }
}
