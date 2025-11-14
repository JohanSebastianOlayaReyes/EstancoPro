/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        // Primarios - Verde Estanco
        'estanco': {
          'dark': '#1B3A1B',
          DEFAULT: '#2D5A2D',
          'light': '#3D7A3D',
          'lighter': '#4A9A4A',
        },
        // Acentos
        'lime': {
          DEFAULT: '#7CB342',
          'light': '#689F38',
        },
        'sage': '#8BC34A',
        'mint': '#A5D6A7',
        'border-green': '#C8E6C9',
        // Fondos
        'bg-green': {
          DEFAULT: '#F1F8F4',
          'light': '#F8FBF9',
        },
        // Estados
        'success': {
          DEFAULT: '#43A047',
          'light': '#66BB6A',
          'bg': '#E8F5E9',
        },
        'warning': {
          DEFAULT: '#FBC02D',
          'light': '#FDD835',
          'bg': '#FFFDE7',
        },
        'error': {
          DEFAULT: '#E53935',
          'light': '#EF5350',
          'bg': '#FFEBEE',
        },
        'info': {
          DEFAULT: '#039BE5',
          'light': '#29B6F6',
          'bg': '#E1F5FE',
        },
        // Financieros
        'cash': {
          'green': '#4CAF50',
          'green-light': '#81C784',
          'green-bg': '#E8F5E9',
        },
        'expense': {
          'red': '#EF5350',
          'red-light': '#E57373',
          'red-bg': '#FFEBEE',
        },
        'profit': {
          'gold': '#FFB300',
          'gold-light': '#FFCA28',
          'gold-bg': '#FFF8E1',
        },
      },
      boxShadow: {
        'green-sm': '0 2px 4px rgba(45, 90, 45, 0.1)',
        'green-md': '0 4px 8px rgba(45, 90, 45, 0.12)',
        'green-lg': '0 8px 16px rgba(45, 90, 45, 0.15)',
        'lime': '0 4px 12px rgba(124, 179, 66, 0.3)',
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        mono: ['Fira Code', 'Courier New', 'monospace'],
      },
    },
  },
  plugins: [],
}
