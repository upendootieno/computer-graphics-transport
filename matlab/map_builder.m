% read OSM XML into MATLAB structure
maps_path = "/home/voyager/Desktop/master/work/projects/toka-kwa-block/maps/"
xml = xmlread(maps_path + 'the_shoe_planet.osm');

% get node elements (lat/lon)
nodeList = xml.getElementsByTagName('node');
numNodes = nodeList.getLength;

ids = zeros(numNodes,1);
lats = zeros(numNodes,1);
lons = zeros(numNodes,1);

for i = 1:numNodes
    node = nodeList.item(i-1);
    ids(i)  = str2double(node.getAttribute('id'));
    lats(i) = str2double(node.getAttribute('lat'));
    lons(i) = str2double(node.getAttribute('lon'));
end

% Build a map id->index (for edges later)
id2idx = containers.Map(ids, 1:numNodes);


%% === DFS (manual tree building, version-safe) ===

startNode = 1;

visited  = false(numnodes(G),1);
stack    = startNode;
treeEdges = [];

while ~isempty(stack)
    u = stack(end);
    stack(end) = [];

    if ~visited(u)
        visited(u) = true;
        nbrs = neighbors(G,u);

        for v = nbrs'
            if ~visited(v)
                treeEdges = [treeEdges; u v];  % DFS tree edge
                stack = [stack; v];            % push
            end
        end
    end
end

T = graph(treeEdges(:,1), treeEdges(:,2));

% Tree Visualization
figure;
h = plot(T,'Layout','layered');
title('DFS Tree (manual)');

