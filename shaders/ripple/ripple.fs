#version 330

out vec4 color; // Current pixel color
uniform float time; // Create float time
uniform vec2 resolution; // Get resolution from Effect Launcher.cs

const vec2 centre = vec2(0.5, 0.5); // Center of the ripple
const float speed = 0.035; // Speed of the ripple, higher = faster

void main()
{
    vec2 uv = gl_FragCoord.xy / resolution; // Convert pixel coordinates to 0-1 UV coords

    float aspectRatio = resolution.y / resolution.x; // Correct aspect ratio

    vec3 backColor = vec3(uv.x,uv.y,0.5 + 0.5 * sin(time)); // background color

    float x = centre.x - uv.x;
    float y = (centre.y - uv.y) * aspectRatio; 
    // Distance of current pixel from centre

    float distanceFromCentre = -(x * x + y * y); // Distance ripple is from centre
    
    float ripple = 
    1.0 + 0.5 * sin((distanceFromCentre + time * speed) / 0.013);
    // Creates the actual ripples

    vec3 rippleBrightness = vec3(ripple); // ripple brightness

    vec3 finalColor = backColor * rippleBrightness; 
    color = vec4(finalColor, 1.0);
    // final ripple color
}
