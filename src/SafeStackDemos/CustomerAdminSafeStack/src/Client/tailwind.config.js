/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./src/**/*.{fs,fsx,html,js,jsx,ts,tsx}",
        "./public/index.html"
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require('daisyui')
    ],
    daisyui: {
        themes: ["light", "dark"], // or any daisyUI themes you want
    }
}
