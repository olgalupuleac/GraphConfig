using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using GraphConfiguration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
        private Debugger _debugger;

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var applicationObject = (DTE) Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            _debugger = applicationObject.Debugger;
            CreateConfig();
            RenderGraph();
        }

        private void CreateConfig()
        {
            NodeFamily nodes = new NodeFamily(
                new List<IdentifierPartTemplate>()
                {
                    new IdentifierPartTemplate("v", "0", "n")
                }
            );
            EdgeFamily edges = new EdgeFamily(
                new List<IdentifierPartTemplate>
                {
                    new IdentifierPartTemplate("a", "0", "n"),
                    new IdentifierPartTemplate("b", "0", "n"),
                    new IdentifierPartTemplate("x", "0", "g[__a__].size()")
                }, "__a__", "__b__"
            );
            edges.Properties.Add(Tuple.Create(new Condition("g[__a__][__x__] == __b__"),
                (IEdgeProperty) new ValidationEdgeProperty()));
            _config = new GraphConfig
            {
                Edges = new HashSet<EdgeFamily> {edges},
                Nodes = new HashSet<NodeFamily> {nodes}
            };
        }

        private void RenderGraph()
        {
            foreach (var edgeFamily in _config.Edges)
            {
                foreach (var partTemplate in edgeFamily.Ranges)
                {

                }
            }
        }
    }
}