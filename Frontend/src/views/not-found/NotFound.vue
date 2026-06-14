<template>
  <div class="not-found-container">
    <div class="not-found-content">
      <div class="code">404</div>
      <div class="message">Không tìm thấy trang</div>
      <router-link to="/" class="home-link">Quay lại trang chủ</router-link>
    </div>
  </div>
</template>

<style scoped>
.not-found-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f2f2f2;
}

.not-found-content {
  text-align: center;
}

.code {
  font-size: 100px;
  font-weight: bold;
  color: #2680eb;
}

.message {
  font-size: 24px;
  margin-bottom: 20px;
  color: #111;
}

.home-link {
  color: #2680eb;
  text-decoration: none;
  font-weight: 500;
}

.home-link:hover {
  text-decoration: underline;
}
</style>
