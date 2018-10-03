import sys

import matplotlib.pyplot as plt
import matplotlib.patches as patches
from matplotlib import cm
from numpy import linspace


if len(sys.argv) != 2:
	print("Please provide only the full filename and path to data that should be plotted")
	sys.exit()
data = []
with open(sys.argv[1]) as file:
	for line in file:
		line = line.strip(" {};"+'\n')
		line.strip()
		points = line.split(';')
		data.append([])
		for point in points:
			point = point.strip("()");
			p = point.split(",")
			data[-1].append((float(p[0]),float(p[1])))

means = data.pop(0)
thresholds = data.pop(0)
thresholds = [(x[0]*2,x[1]) for x in thresholds]
cm_subsection = linspace(0.0, 1.0, len(data)+1) 
colors = [ cm.jet(x) for x in cm_subsection ]

for i in range(len(data)):
	x_val = [x[0] for x in data[i]]
	y_val = [x[1] for x in data[i]]
	plt.scatter(x_val,y_val, color=colors[i],alpha=0.5, label="State "+str(i))

x_val = [x[0] for x in means]
y_val = [x[1] for x in means]
plt.scatter(x_val,y_val,marker="X", s=80, color=colors[-1], label="State Center")

for i in range(len(means)):
	thresh = patches.Rectangle((means[i][0]-thresholds[i][0]/2,means[i][1]-thresholds[i][1]/2),thresholds[i][0],thresholds[i][1],linewidth=1, edgecolor=colors[i],facecolor='none')
	plt.gca().add_patch(thresh)
plt.axis('scaled')
plt.show()