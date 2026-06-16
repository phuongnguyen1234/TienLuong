import Prism from 'prismjs'
import 'prismjs/themes/prism.css'

// Cấu hình ngôn ngữ formula một lần duy nhất
if (!Prism.languages.formula) {
  Prism.languages.formula = {
    variable: {
      pattern: /\[[a-zA-Z0-9_]+\]/,
      alias: 'property',
    },
    operator: /[+\-*/()]/,
    number: /\b\d+(\.\d+)?\b/,
  }
}

/**
 * Hàm highlight công thức lương
 * @param {string} code Chuỗi công thức
 * @returns {string} Chuỗi HTML đã được highlight
 */
export const highlightFormula = (code) => {
  if (!code) return ''
  return Prism.highlight(code, Prism.languages.formula, 'formula')
}
