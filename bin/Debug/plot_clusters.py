import sys

import matplotlib.pyplot as plt
from matplotlib import cm
from numpy import linspace


if len(sys.argv) != 2:
	print("Please provide only the full filename and path to data that should be plotted")
	sys.exit()
clusters = []
with open(sys.argv[1]) as file:
	for line in file:
		line = line.strip(" {};"+'\n')
		line.strip()
		points = line.split(';')
		clusters.append([])
		for point in points:
			point = point.strip("()");
			p = point.split(",")
			clusters[-1].append((float(p[0]),float(p[1])))

cm_subsection = linspace(0.0, 1.0, len(clusters)+1) 
colors = [ cm.jet(x) for x in cm_subsection ]
for i in range(len(clusters)-1):
	x_val = [x[0] for x in clusters[i]]
	y_val = [x[1] for x in clusters[i]]
	plt.scatter(x_val,y_val, color=colors[i], label="Cluster "+str(i))

x_val = [x[0] for x in clusters[len(clusters)-1]]
y_val = [x[1] for x in clusters[len(clusters)-1]]
plt.scatter(x_val,y_val,marker="X", s=80, color=colors[-1], label="Cluster Center")

plt.show()