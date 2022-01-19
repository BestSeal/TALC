import networkx as nx
import matplotlib.pyplot as plt
import json

serializedStates = open("D:\\dev\\c#\\TALC\\Lab2\\BlackMagic\\states.txt").read();
data = json.loads(serializedStates)

def nudge(pos, x_shift, y_shift):
    return {n:(x + x_shift, y + y_shift) for n,(x,y) in pos.items()}

G = nx.DiGraph()

labels = dict()

for state in data:
    nextStates = state["NextStates"]
    if len(nextStates) == 0:
        G.add_node(state["Name"])
    else:
        for nextState in nextStates:
            G.add_edge(state["Name"], nextState["Item2"])
            labels[(state["Name"], nextState["Item2"])] = f'\"{nextState["Item1"]}\"'

pos = nx.kamada_kawai_layout(G)

nx.draw(G, pos, with_labels=True)
nx.draw_networkx_edge_labels(G, nudge(pos, 0, 0.1), edge_labels=labels)

ax = plt.gca()
ax.margins(0.20)
plt.axis("off")
plt.savefig("D:/dev/c#/TALC/Lab2/BlackMagic/graph.png")
print("done")
