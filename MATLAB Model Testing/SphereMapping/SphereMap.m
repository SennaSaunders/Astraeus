clc
clear

data = GetData();

for i = 0:max(data(:,3))
    rowDataIdx(i+1,:) = find(data(:,3) == i)
end
scatter3(data(:,1), data(:,2), data(:,3),".");
xlabel("X")
ylabel("Y")
zlabel("Z")
% sphereRadius = 32;
% z = -3
% circleRadius = GetChordRadius(sphereRadius,z)
% test = GetCircleCoordinates(sphereRadius, z, circleRadius);
% plot3(test(:,1), test(:,2), test(:,3), "x");

function [data] = GetData()
    dataString = "SphereMappings.txt";    
    data = readmatrix(dataString);
end

function [circleCoordinates] = GetCircleCoordinates(sphereRadius, z, circleRadius)
    i = 1;
    while i  < (circleRadius*2)
        theta = (i/(circleRadius*2))* 2 * pi
        x = sphereRadius * sin(theta);
        y = sphereRadius * cos(theta);
        circleCoordinates(i, :) = [x,y,z];
        i = i + 1
    end
end

function [circleRadius] = GetChordRadius(sphereRadius, d)
    circleRadius = sqrt(sphereRadius*sphereRadius - d * d);
end
