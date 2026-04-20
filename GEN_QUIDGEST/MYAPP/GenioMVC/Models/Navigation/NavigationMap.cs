using System;
using System.Collections.Generic;

namespace GenioMVC.Models.Navigation
{
	public class NavigationMap
	{
		private class NavigationMapNode
		{
			public NavigationLocation Location { get; set; }

			private List<NavigationMapNode> m_origins;
			public IEnumerable<NavigationMapNode> Origins
			{
				get
				{
					return m_origins.AsReadOnly();
				}
			}

			private List<NavigationMapNode> m_destinations;
			public IEnumerable<NavigationMapNode> Destinations
			{
				get
				{
					return m_destinations.AsReadOnly();
				}
			}

			private NavigationMapNode()
			{
				m_origins = new List<NavigationMapNode>();
				m_destinations = new List<NavigationMapNode>();
			}

			public NavigationMapNode(NavigationLocation location) : this()
			{
				Location = location;
			}

			public void AddDestination(NavigationMapNode destination)
			{
				if (!m_destinations.Contains(destination))
				{
					m_destinations.Add(destination);
					destination.m_origins.Add(this);
				}
			}

			public void RemoveDestination(NavigationMapNode destination)
			{
				if (destination == null)
					throw new ArgumentNullException("destination");

				m_destinations.Remove(destination);
				destination.m_origins.Remove(this);
			}

			public bool ContainsDestination(NavigationMapNode destination)
			{
				if (destination == null)
					throw new ArgumentNullException("destination");

				return m_destinations.Contains(destination);
			}
		}

		private IDictionary<NavigationLocation, NavigationMapNode> Nodes { get; set; }

		public NavigationMap()
		{
			Nodes = new Dictionary<NavigationLocation, NavigationMapNode>();
		}

		public void AddLink(NavigationLocation origin, NavigationLocation destination)
		{
			NavigationLocation pOrigin = origin.Normalize();
			NavigationLocation pDestination = destination.Normalize();

			NavigationMapNode originNode = GetOrCreateNode(pOrigin);
			NavigationMapNode destinationNode = GetOrCreateNode(pDestination);

			originNode.AddDestination(destinationNode);
		}

		public void RemoveLink(NavigationLocation origin, NavigationLocation destination)
		{
			NavigationLocation pOrigin = origin.Normalize();
			NavigationLocation pDestination = destination.Normalize();

			NavigationMapNode originNode = GetOrCreateNode(pOrigin);
			NavigationMapNode destinationNode = GetOrCreateNode(pDestination);

			originNode.RemoveDestination(destinationNode);
		}

		public bool ExistsLink(NavigationLocation origin, NavigationLocation destination)
		{
			NavigationLocation pOrigin = origin.Normalize();
			NavigationLocation pDestination = destination.Normalize();

			if (pOrigin == pDestination)
				return true;
			else if (Nodes.ContainsKey(pOrigin) && Nodes.ContainsKey(pDestination))
				return Nodes[pOrigin].ContainsDestination(Nodes[pDestination]);
			else if (Nodes.ContainsKey(NavigationLocation.Any) && Nodes.ContainsKey(pDestination))
				return Nodes[NavigationLocation.Any].ContainsDestination(Nodes[pDestination]);
			return false;
		}

		private NavigationMapNode GetOrCreateNode(NavigationLocation location)
		{
			if (!Nodes.ContainsKey(location))
				Nodes.Add(location, new NavigationMapNode(location));

			return Nodes[location];
		}
	}
}
