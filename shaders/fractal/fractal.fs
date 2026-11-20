#version 330

out vec4 color;
uniform float time;
uniform vec2 resolution;

vec2 rotate(vec2 p, float a) // Rotates the current point
{
    float c = cos(a);
    float s = sin(a);
    return p * mat2(c, s, -s, c);
}

float map(vec3 p) 
// Continiously rotates, folds and shifts the current point to create the twisting pattern visible
{
    for( int i = 0; i<8; ++i)
    {
        float t = time * 0.2; // Rotation speed (higher = faster)
        p.xz = rotate(p.xz, t); // Rotates x and Z
        p.xy = rotate(p.xy, t * 1.89); // Rotates x and Y 
        p.xz = abs(p.xz); // Folds X and Z
        p.xz-= .5; // Shifts X and Z by .5 (makes the shape expand outward)
	}
	return dot(sign(p),p)/5.; // Returns final value
}

vec4 rm(vec3 ro, vec3 ray) // Raymarching loop
{
    float t = 0.0;
    vec3 col = vec3(0.0);

    for (int i = 0; i < 64; ++i) 
    {
        vec3 p = ro + ray * t; // Current position
        float d = map(p); // Current fractal pattern

        // Color of the fractal / tint
        vec3 tint = mix(vec3(0.2, 0.7, 0.9), vec3(1.0, 0.0, 1.0), length(p) * 0.1);
        col += tint / (400.0 * d);
        t += d;
    }

    return vec4(col, 1.0);
}

void main()
{
    vec2 uv = (gl_FragCoord.xy - resolution * 0.5) / resolution.x; // Normalise coords

    // Orbits the camera around the origin
    vec3 ro = vec3(0.0, 0.0, -50.0);
    ro.xz = rotate(ro.xz, time);

    // Builds the camera's forward, sideways, and vertical directions
    vec3 cf = normalize(-ro);
    vec3 cs = normalize(cross(cf, vec3(0.0, 1.0, 0.0)));
    vec3 cu = normalize(cross(cf, cs));

    // Find current point and send ray that direction
    vec3 uuv = ro + cf * 3.0 + uv.x * cs + uv.y * cu;
    vec3 rd = normalize(uuv - ro);

    color = rm(ro, rd); // Adds color to current pixel
}