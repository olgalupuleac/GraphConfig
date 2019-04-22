using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;
using EnvDTE;
using GraphConfiguration.Config;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.VisualStudio.Shell;
using GraphRenderer = GraphConfiguration.GraphRenderer.GraphRenderer;
using Task = System.Threading.Tasks.Task;

namespace GraphPlugin
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class RenderGraphCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("1548d1ac-7f09-47e1-9665-c1995dfa872f");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderGraphCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private RenderGraphCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var applicationObject = (DTE) Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            _debugger = applicationObject.Debugger;
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static RenderGraphCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get { return this.package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in RenderGraphCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService =
                await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new RenderGraphCommand(package, commandService);
        }

        private GraphConfig _config;
        private readonly Debugger _debugger;
        private Dictionary<string, Edge> _edges = new Dictionary<string, Edge>();

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            CreateConfig();
            GraphRenderer renderer = new GraphRenderer(_config,
                _debugger);
            Graph graph = renderer.RenderGraph();
            GViewer viewer = new GViewer {Graph = graph, Dock = DockStyle.Fill};
            Form form = new Form();
            form.SuspendLayout();
            form.Controls.Add(viewer);
            form.ResumeLayout();
            form.Show();
        }

        private void CreateConfig()
        {
            var visitedNode = System.Tuple.Create(
                new Condition("!strcmp(\"__CURRENT_FUNCTION__\", \"dfs\") && visited[__v__]"),
                (INodeProperty)new FillColorNodeProperty(Color.Green));
            var dfsNode = System.Tuple.Create(
                new Condition("!strcmp(\"__CURRENT_FUNCTION__\", \"dfs\") && __ARG1__ == __v__", true),
                (INodeProperty) new FillColorNodeProperty(Color.Gray));
            var currentNode = System.Tuple.Create(
                new Condition("!strcmp(\"__CURRENT_FUNCTION__\", \"dfs\") && __ARG1__ == __v__"),
                (INodeProperty) new FillColorNodeProperty(Color.Red));


            NodeFamily nodes = new NodeFamily(
                new List<IdentifierPartTemplate>()
                {
                    new IdentifierPartTemplate("v", "0", "n")
                }
            );
            nodes.Properties.Add(visitedNode);
            nodes.Properties.Add(dfsNode);
            nodes.Properties.Add(currentNode);
            EdgeFamily edges = new EdgeFamily(
                new List<IdentifierPartTemplate>
                {
                    new IdentifierPartTemplate("a", "0", "n"),
                    new IdentifierPartTemplate("x", "0", "n")
                }, new EdgeFamily.EdgeEnd(nodes, new List<string> {"__a__"}),
                new EdgeFamily.EdgeEnd(nodes, new List<string> {"g[__a__][__x__]"})
            ) {ValidationTemplate = "__x__ < g[__a__].size()", IsDirected = true};
            var dfsEdges = Tuple.Create(
                new Condition("p[g[__a__][__x__]].first == __a__ && p[g[__a__][__x__]].second == __x__"),
                (IEdgeProperty) new LineColorEdgeProperty(Color.Red));
            edges.Properties.Add(dfsEdges);
            _config = new GraphConfig
            {
                Edges = new HashSet<EdgeFamily> {edges},
                Nodes = new HashSet<NodeFamily> {nodes}
            };
        }
    }
}