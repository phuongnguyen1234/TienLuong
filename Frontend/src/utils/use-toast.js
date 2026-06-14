export function useToast() {
  /**
   * Hiển thị một thông báo toast.
   * @param {string} message - Nội dung thông báo.
   * @param {string} type - Loại thông báo ('success' hoặc 'error').
   * @param {number} duration - Thời gian hiển thị (ms).
   */
  const showToast = (message, type = 'success', duration = 3000) => {
    const toastContainer = document.getElementById('toast')
    if (!toastContainer) {
      console.error('Toast container with id "toast" not found in the DOM.')
      return
    }

    const toast = document.createElement('div')
    toast.className = `toast toast--${type}`

    // Tạo icon
    const icon = document.createElement('i')
    const iconClass = type === 'success' ? 'fi fi-rr-check-circle' : 'fi fi-rr-cross-circle'
    icon.className = `${iconClass} toast_icon`

    // Tạo message
    const msg = document.createElement('div')
    msg.className = 'toast_msg'
    msg.innerText = message

    // Tạo nút đóng (sử dụng SVG từ CloseIcon)
    const closeBtn = document.createElement('div')
    closeBtn.className = 'toast_close'
    closeBtn.innerHTML = `
      <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M13 3L3 13M3 3L13 13" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
    `
    closeBtn.onclick = () => {
      toast.remove()
    }

    // Thêm các thành phần vào toast
    toast.appendChild(icon)
    toast.appendChild(msg)
    toast.appendChild(closeBtn)

    toastContainer.appendChild(toast)

    setTimeout(() => {
      if (toast.parentNode) toast.remove()
    }, duration + 500) // Cộng thêm 500ms để chờ hiệu ứng slideOutUp hoàn tất
  }

  return { showToast }
}
