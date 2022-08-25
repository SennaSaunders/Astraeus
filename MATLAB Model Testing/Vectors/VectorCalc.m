clc
clear
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Parameters
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

velocity = [-250 -1 0];
destination = [0 0 0];
pos = [-1900 -7 0];
facingDeg = 90;
facing = facingDeg/180 * pi;
accel = [38 15 0];

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Finding thrust differential
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

distanceToDest = destination-pos
dot = dot(velocity, distanceToDest);
cross = cross(velocity,distanceToDest);
angleRads = atan2(norm(cross), dot);
angleDeg = angleRads * (180/pi);

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Finding ship thrust vector
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

Rz = [cos(facing) -sin(facing) 0;sin(facing) cos(facing) 0; 0 0 1];
shipRotatedV = velocity * Rz'
% calculate if it reaches the destination slowing from current velocity
shipRotatedDistanceToDest = distanceToDest * Rz';
vx = shipRotatedV(1);
vy = shipRotatedV(2);
sx = (vx*vx)/(2*(accel(1)));
sy = (vy*vy)/(2*(accel(2)));
slowDownVec = [sx sy 0]
distVDelta = shipRotatedDistanceToDest-slowDownVec
thrustVec = distVDelta./abs(distVDelta)

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Accounting for slowdown
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% requiredV = shipRotatedDistanceToDest-shipRotatedV
% xVec = requiredV(1)/abs(requiredV(1));
% yVec = requiredV(2)/abs(requiredV(2));
% thrustVec = [xVec yVec 0]%i think we can stop here in terms of the thrust stuff we need to do
% accelVec = thrustVec .* accel

% s=(vf^2-vi^2)/2a
% vf = 0 so
% s=(-vi^2)/2a
% s = (-(requiredV.*requiredV)./accelVec);
% s(isnan(s))=0
