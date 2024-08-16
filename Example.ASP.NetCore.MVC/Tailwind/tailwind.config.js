/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["**/*.cshtml"],
    theme: {
        extend: {},
    },
    plugins: [
        require('@tailwindcss/forms'),
        require('@tailwindcss/typography'),
    ],
}
