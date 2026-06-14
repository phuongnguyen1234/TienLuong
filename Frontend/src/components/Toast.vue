<template>
  <!-- Toast Container -->
  <div id="toast"></div>
</template>

<style>
/* Toast message styles */
#toast {
  position: fixed;
  top: 36px;
  left: 50%;
  transform: translateX(-50%);
  z-index: var(--z-index-toast);
  display: flex;
  flex-direction: column;
  align-items: center;
}

.toast {
  display: flex;
  align-items: center;
  min-width: 150px;
  max-width: 400px;
  min-height: 48px;
  padding: 8px 12px;
  border-radius: 8px;
  color: #fff;
  margin-bottom: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  animation:
    slideInDown 0.4s ease,
    slideOutUp 0.5s 3s forwards;
  font-family: 'Inter', sans-serif;
  font-size: 13px;
  box-sizing: border-box;
}

.toast_icon {
  width: 20px;
  height: 20px;
  margin-right: 12px;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.toast_msg {
  flex-grow: 1;
  padding-right: 8px;
  line-height: 1.4;
}

.toast_close {
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  color: #fff;
  opacity: 0.8;
  transition: opacity 0.2s;
}

.toast_close:hover {
  opacity: 1;
}

.toast--success {
  background-color: var(--color-success);
}

.toast--warning {
  background-color: var(--color-warning);
}

.toast--error {
  background-color: var(--color-error);
}

@keyframes slideInDown {
  from {
    opacity: 0;
    transform: translateY(-100%);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes slideOutUp {
  from {
    opacity: 1;
    transform: translateY(0);
  }
  to {
    opacity: 0;
    transform: translateY(-100%);
  }
}
</style>
