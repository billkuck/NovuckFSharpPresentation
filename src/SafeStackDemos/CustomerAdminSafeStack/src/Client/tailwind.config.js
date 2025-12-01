/** @type {import('tailwindcss').Config} */
module.exports = {
    mode: "jit",
    content: [
        "./index.html",
        "./**/*.{fs,js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {},
        screens: {
            'sm': '640px',
            // => @media (min-width: 640px) { ... }

            'md': '768px',
            // => @media (min-width: 768px) { ... }

            'lg': '1024px',
            // => @media (min-width: 1024px) { ... }`

            'xl': '1280px',
            // => @media (min-width: 1280px) { ... }

            '2xl': '1536px',
            // => @media (min-width: 1536px) { ... }
        }
    },

    plugins: [require("@tailwindcss/typography"), require("daisyui")],

    // daisyUI config (optional - here are the default values)

    daisyui: {
        themes: ["light",
            "dark",
            "cupcake",
            "bumblebee",
            "emerald",
            "corporate",
            "synthwave",
            "retro",
            "cyberpunk",
            "valentine",
            "halloween",
            "garden",
            "forest",
            "aqua",
            "lofi",
            "pastel",
            "fantasy",
            "wireframe",
            "black",
            "luxury",
            "dracula",
            "cmyk",
            "autumn",
            "business",
            "acid",
            "lemonade",
            "night",
            "coffee",
            "winter",
            "dim",
            "nord",
            "sunset",],
        darkTheme: "dark", // name of one of the included themes for dark mode
        base: true, // applies background color and foreground color for root element by default
        styled: true, // include daisyUI colors and design decisions for all components
        utils: true, // adds responsive and modifier utility classes
        prefix: "", // prefix for daisyUI classnames (components, modifiers and responsive class names. Not colors)
        logs: true, // Shows info about daisyUI version and used config in the console when building your CSS
        themeRoot: ":root", // The element that receives theme color CSS variables
    },
}
