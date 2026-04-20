export const PathUtils={
	//----------------
//The methods in this class assume the node objects will have the following internal structure:
//	[{name:_, imports:[{source, type, link} ...]} ...]
//----------------

/**
 * Represents a forward dependency
 * @typedef {Object} DependencyNode
 * @property {string} name
 * @property {Array[]} imports
 */

/**
 * Represents a graph of forward dependencies
 * @typedef {DependencyNode[]} DependencyGraph
 */

//----------------------

/**
 * Calculates the nodes that participate in all the paths between the start node and the end node in a a graph
 * @param {DependencyGraph} graph A graph consisting of an array of nodes
 * @param {string} start The starting node
 * @param {string} end The target node
 * @returns {string[]} An array with all the nodes participating in the path. The array will be empty if no path was found.
 */
 StartNodeSpan:function(graph, start, end)
{
	var participant = [];
	var visited = [];
	PathUtils.__RecurseNodeSpan(graph, start, end, participant, visited);
	
	return participant;
},

/**
 * Private recursive function for exclusive use by StartNodeSpan
 * @param {DependencyGraph} graph The dependency graph
 * @param {string} node The node to span
 * @param {string} end The target node
 * @param {string[]} participant An array with all the nodes participating in the path.
 * @param {string[]} visited  An array with all the nodes already visited
 */
__RecurseNodeSpan: function (graph, node, end, participant, visited)
{
	//console.log("__RecurseNodeSpan", node);
	
	if(visited.indexOf(node) != -1)
		return (participant.indexOf(node) != -1);

	var p = false;
	var nodeIx = -1;
	graph.forEach(function(n, ix) { if(n.name == node) nodeIx = ix;});
	
	if(nodeIx == -1)
		return false;	
	
	visited.push(node);

//console.log(graph[nodeIx]);

	graph[nodeIx].imports.forEach(function(child) {
		if(PathUtils.__RecurseNodeSpan(graph, child[0], end, participant, visited))
			p = true;
	});
	if(node == end)
		p = true;
		
	
	if(p)
		participant.push(node);
		
	return p;
},


//Looks for imports that are not in the nodes and creates an empty node as a placeholder
//The array will be modified in place so this function has no return value
//@classes: And array of nodes
expandClasses:function (classes) {
	var map = {};
	
	classes.forEach(function(c) {
		map[c.name] = c;
	});
	
	classes.forEach(function(c) {
		c.imports.forEach(function(i){
			var node = map[i[0]];
			if(!node)
			{
				node = { name: i[0], imports: []};
				map[i[0]] = node;
				classes.push(node);
			}
		});
	});
},


/**
 * Given a vector and a selection it returns the inverse selection, i.e. all the nodes that are not selected
 * @param {DependencyGraph} graph The full vector of elements
 * @param {string[]} selection The current selection
 * @return {string[]} The inverted selection
 */
 invertSelection:function (graph, selection)
{
	var res = [];
	for (let i = 0; i < graph.length; i++) {
		if(selection.indexOf(graph[i].name) === -1)
			res.push(graph[i].name);
	}
	return res;
},


/**
 * Given a starting node expands forward and backwards to all the reachable nodes in a graph.
 * Additinally the node objects will be marked with the attribute 'class' with value 'sources' or 'dests' according to how they where expanded.
 * The starting node will the marked with class 'origin'.
 * @param {DependencyGraph} graph The complete graph
 * @param {string} node The starting node id
 * @param {boolean=} singleBack True if only one backwards connection is expanded
 * @returns {DependencyGraph} A new graph containing all the reachable node objects
 */
 expandNode:function (graph, node, singleBack) {
	//calculate sources propagation
	var newNodes = [];
	var visited = {};

	//check if node exists
	if(!graph.find(function(x) { return x.name == node;}))
		return newNodes;

	//expand forward recursively
	PathUtils.__filterFormRecurseSources(graph, node, newNodes, visited);
	newNodes.forEach(function(n) { n.class = "dests" });	

	//calculate destinations propagation
	var backQueue = [];
	if(singleBack) {
		graph.forEach(function(n) {
			if(n.imports.find(function(x) { return x[0] == node;}) )
				backQueue.push(n);
		});
	}
	else {
		backQueue = PathUtils.__filterFormRecurseDests(graph, node);
	} 
	backQueue.forEach(function(n) { n.class = "sources" });
	
	//merge the two lists
	newNodes[0].class = "origin";
	backQueue.forEach(function(n) {
		if(!newNodes.find(function(x) { return x.name == n.name;} ))
			newNodes.push(n);
	});
	
	return newNodes;
},

//private function for filterForm
__filterFormRecurseDests:function (nodes, form) {
	var visited = {};
	var backQueue = [];
	
	var core = nodes.find(function(elem) {return elem.name == form;});
	if(core == null)
		return backQueue;

	backQueue.push(core);
	visited[form] = true;
	var oldcount = 0;
	var newcount = 1;
	while(oldcount < newcount)
	{
		oldcount = newcount;
		
		nodes.forEach(function(n) {
			if(!visited[n.name]) {
				var found = false;
				for(var i=0; i< backQueue.length; i++) {
					if(n.imports.find(function(x) { return x[0] == backQueue[i].name;}))
					{
						found = true;
						break;
					}
				}
				if(found)
				{
					backQueue.push(n);
					visited[n.name] = true;
					//newNodes.push(n);
					//console.log(n.name);
				}
			}
		});
		
		newcount = backQueue.length;
	}
	return backQueue;
},


//private function for filterForm
__filterFormRecurseSources: function (nodes, form, newNodes, visited) {
	var core = nodes.find(function(elem) {return elem.name == form;});
	if(!core) return;
	
	newNodes.push(core);
	visited[form] = true;
	
	core.imports.forEach(function(c){
		var cname = c[0];
		if(!visited[cname])
			PathUtils.__filterFormRecurseSources(nodes, cname, newNodes, visited);
	});
},

/**
 * Reacheability matrix
 * @typedef {Object.<string[]>} ReacheabilityMatrix
 */

/**
 * Computes a reacheability matrix for a graph. Each node will be added a list of all the other nodes it can reach
 * @param {DependencyGraph} graph The graph to analyse
 * @returns {ReacheabilityMatrix} The reacheability matrix
 */
reachabilityMatrix:function (graph)
{
	var matrix = {};
	
	graph.forEach(function(n){
		PathUtils.__reacheabilityRecurse(graph, matrix, n, []);
	});
	
	return matrix;
},

//private function to recurse reachabilityMatrix
__reacheabilityRecurse:function (graph, matrix, n, cycle_prot)
{
	//already calculated
	if(matrix[n.name]) return;
	var dests = [];
	cycle_prot.push(n.name); //we are in the middle of the calculation
	
	//add our own dests
	n.imports.forEach(i=> {
	
		var impname = (i.Designation);
	
		//ensure the destination of our dest are calculated (recursive)
		var dnode = graph.find(function(x) { return x.name == impname; });
	
		//discard unknown nodes
		if(dnode)
		{
			//console.log("matrix", n.name, impname, cycle_prot);
			if(dests.indexOf(impname) == -1)
				dests.push(impname);

			//avoid cycles in the graph
			if(cycle_prot.indexOf(impname) == -1)
			{
				PathUtils.__reacheabilityRecurse(graph, matrix, dnode, cycle_prot);
		
				//add the dests of this dest to our own
				matrix[impname].forEach(function(d) {
					if(dests.indexOf(d) == -1)
						dests.push(d);
				});
			}
		}
	});
	//cache our result
	matrix[n.name] = dests;
},

/**
 * Determines if this is an edge that cannot be redundant given another edge
 * An edge is primary if a node X does not exist such that:
 *  1. src reaches X
 *  2. X reaches dst
 *  3. X!=src && X!=dst
 *  (for cycles)
 *  4. dst does not reach X
 *  5. X does not reach src
 * @param {ReacheabilityMatrix} reachMatrix Precalculated reacheability matrix
 * @param {string} src Source id
 * @param {string} dst Destination id
 */
 isPrimaryEdge:function (reachMatrix, src, dst)
{
	var primary = true;
	for(var i=0; i< reachMatrix[src].length; i++)
	{
		var x = reachMatrix[src][i];
		if(x != dst 
			&& reachMatrix[x].indexOf(dst) > -1 
			&& reachMatrix[dst].indexOf(x) == -1 
			&& reachMatrix[x].indexOf(src) == -1 
		)
		{
			primary = false;
			break;
		}
	}
	
	return primary;
},

/*
//Finds the first object for which the condition is true in an array
//@array: The array to search
//@f: A function that receive each element of the array and returns true or false
//return: The first object in the array for which the condition is true
function findObject(array, f)
{
	for(var i =0; i < array.length; i++)
		if(f(array[i]))
			return array[i];
			
	return null;
}
*/
/*
//Finds all the objects in an array for which the condition is true
//@array: The array to search
//@f: A function that receive each element of the array and returns true or false
//return: All the objects in the array for which the condition is true
function findAllObjects(array, f)
{
	var res = [];
	for(var i =0; i < array.length; i++)
		if(f(array[i]))
			res.push(array[i]);
			
	return res;
}
*/

/**
 * Switches a backwards dependency graph to a forward dependency graph
 *  effectively switching the direction of the dependencies
 * @param {DependencyGraph} graph The input graph
 * @returns {DependencyGraph} The modified graph
 */
switchModelDirection:function (graph) {
	var tempmap = {};
	graph.forEach(function(n) {
		tempmap[n.name] = {name: n.name, imports: []};
	});

	graph.forEach(function(n) {
		n.imports.forEach(function(imp) {
			var name = imp[0];
			var obj = tempmap[name];
			if(obj) {
				var value = [n.name, imp[1], imp[2]];
				obj.imports.push(value);
			}
		});
	});

	var res = [];
	for(let x in tempmap) {
		res.push(tempmap[x]);
	}
	return res;
},

/**
 * Switches the empty dependencies of a back graph to a new forward dependency graph
 *  effectively separating the dependencies without support nodes
 * @param {DependencyGraph} graph The input graph
 * @returns {DependencyGraph} The modified graph
 */
 switchModelEmpties:function (graph) {
	var tempmap = {};

	var existing = new Set();
	graph.forEach(function(n) {
		existing.add(n.name);
	});

	graph.forEach(function(n) {
		n.imports.forEach(function(imp) {
			var name = imp[0];
			if(!existing.has(name))
			{
				var obj = tempmap[name];
				var value = [n.name, imp[1], imp[2]];				
				if(!obj) {
					obj = {name: name, imports: []};
					tempmap[name] = obj;
				}
				obj.imports.push(value);
			}
		});
	});

	var res = [];
	for(let x in tempmap) {
		res.push(tempmap[x]);
	}
	return res;
},

//--------------------------------------------------------------
// RelationalModelGraph
//--------------------------------------------------------------

//Draws a relational model graph within a html node
//@model: the relational model to draw
//@div: the html node where the graph will be rendered. Requires both a <sgv> node with a placeholder <g> node.
/*var RelationalModelGraph = function(model, div) {
	
	this.g = new dagreD3.graphlib.Graph().setGraph({});
	this.g.graph().rankdir = "BT";
	this.g.graph().ranker = "network-simplex"; //network-simplex, longest-path, tight-tree
	
	// setup the nodes
	model.forEach( function(i) {
		this.g.setNode(i.name, {label: i.name, class:i.class});
	}, this);
	
	this.model = model;
	this.showAllEdges = false;
	this.reach = reachabilityMatrix(model);	
	this.updateEdges();
	this.selectdNode = this.model[0].name;

	this.svg = div.select("svg"),
    this.inner = this.svg.select("g");

	// Set up zoom support
	var self = this; //d3 substitutes this for the DOM element, so we need to backup the this value into the self variable
	
	// this.zoom = d3.behavior.zoom().on("zoom", function() {
	// 	  self.inner.attr("transform", "translate(" + d3.event.translate + ")" +
	// 								  "scale(" + d3.event.scale + ")");
	// 	});
	//this.svg.call(this.zoom);

	this.zoom = d3.zoom().on("zoom", function() {
		self.inner.attr("transform", d3.event.transform);
		});
	this.svg.call(this.zoom);

	// Create the renderer
	this.updateRender();

	this.normalSize();
	
	//setup the toolbar*/
	/*
	div.select(".fit-to-size").on("click", function() { self.fitSize();});
	div.select(".fit-normal").on("click", function() { self.normalSize();});
	
	div.select(".primary-links").on("click", function() { self.setLinksPrimary();});
	div.select(".all-links").on("click", function() { self.setLinksAll();});
	*/

	//node click handler
	/*this.svg.selectAll("g.node").on("click", this.nodeClickHandler.bind(this));
};


RelationalModelGraph.prototype.updateEdges = function() {
	//TODO: remove only the unnecessary edges 
	
	//remove all the previous edges
	this.g.edges().forEach( function(edge) {
		this.g.removeEdge(edge.v, edge.w);
	}, this);
	
	// setup the edges
	this.model.forEach( function(i) {
		i.imports.forEach( function(src) {
			if(this.g.node(src[0]))
			{
				if(this.showAllEdges || isPrimaryEdge(this.reach, i.name, src[0]))
				{
					var clEdge = "";
					var wgEdge = 1;
					if(src[1].substring(0,1) == "F")
					{
						clEdge = "secLink";
						wgEdge = 0.5;
					}
					
					this.g.setEdge(i.name, src[0], {label:"", lineInterpolate: 'basis', class: clEdge, weight: wgEdge });
				}
			}
		}, this);
	}, this);
	
}


RelationalModelGraph.prototype.setLinksPrimary = function() {
	if(this.showAllEdges)
	{
		this.showAllEdges = false;
		this.updateEdges();
		this.updateRender();
		this.normalSize();
	}
}

RelationalModelGraph.prototype.setLinksAll = function() {
	if(!this.showAllEdges)
	{
		this.showAllEdges = true;
		this.updateEdges();
		this.updateRender();
		this.normalSize();
	}
}

RelationalModelGraph.prototype.updateRender = function() {
	var render = new dagreD3.render();
	// Run the renderer. This is what draws the final graph.
	render(this.inner, this.g);	
}


RelationalModelGraph.prototype.fitSize = function() {
	// Center the graph	
	var initialScale = 0.75;
	//resize to fit
	var initialScale = (this.svg.attr("width") * 1.0) / (this.g.graph().width + 50.0);
	if(initialScale > 0.75)
		initialScale = 0.75;
	this.rescale(initialScale);
};

RelationalModelGraph.prototype.normalSize = function() {
	this.rescale(0.75);
};

RelationalModelGraph.prototype.rescale = function(newScale) {
	// Center the graph
	this.zoom.transform(this.svg, d3.zoomIdentity
		.translate((this.svg.attr("width") - this.g.graph().width * newScale) / 2, 20)
		.scale(newScale)
		);
	  
	//minimize the height of the container
	this.svg.attr('height', this.g.graph().height * newScale + 40);	
};

RelationalModelGraph.prototype.setSelected = function(nodeName) {
	this.g.node(nodeName).elem.classList.add("selected");
};


RelationalModelGraph.prototype.nodeClickHandler = function(d) {
	console.log(d);
	//clear out previous paths
	this.g.nodes().forEach(function(n) {
		this.g.node(n).elem.classList.remove("hidden");
	}, this);
	this.g.edges().forEach(function(n) {
		this.g.edge(n.v, n.w).elem.classList.remove("hidden");
	}, this);

	//clicking on the central node clears everything
	if(d == this.selectdNode)
		return;

	//calculate the new paths
	var span;
	if(this.g.node(d).class == "dests")
		span = StartNodeSpan(this.model, this.selectdNode, d);
	else
		span = StartNodeSpan(this.model, d, this.selectdNode);
	
		console.log(span);
	//hide nodes
	var invspan = invertSelection(this.model, span);
	invspan.forEach(function(n) {
		//console.log(this.g.node(n));
		//if(n != this.selectdNode)
			this.g.node(n).elem.classList.add("hidden");
	}, this);
	
	//hide edges
	this.g.edges().forEach(function(e) {
		if(span.indexOf(e.v)>-1 && span.indexOf(e.w)>-1)
			this.g.edge(e.v, e.w).elem.classList.remove("hidden");
		else
			this.g.edge(e.v, e.w).elem.classList.add("hidden");
	}, this);
};
*/
}