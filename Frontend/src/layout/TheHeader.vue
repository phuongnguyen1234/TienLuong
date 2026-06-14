<template>
  <header class="header">
    <!-- Header Left -->
    <div class="header_left">
      <Tooltip text="Tất cả ứng dụng">
        <div class="header_apps">
          <svg width="24" height="24" viewBox="0 0 24 24">
            <g id="Group_45252" data-name="Group 45252">
              <circle
                id="Ellipse_2436"
                data-name="Ellipse 2436"
                cx="1"
                cy="1"
                r="1"
                transform="translate(17 17)"
              />
              <!-- ... Các circle khác tương tự, bỏ stroke cứng ... -->
              <circle cx="1" cy="1" r="1" transform="translate(11 17)" />
              <circle cx="1" cy="1" r="1" transform="translate(5 17)" />
              <circle cx="1" cy="1" r="1" transform="translate(17 11)" />
              <circle cx="1" cy="1" r="1" transform="translate(11 11)" />
              <circle cx="1" cy="1" r="1" transform="translate(5 11)" />
              <circle cx="1" cy="1" r="1" transform="translate(17 5)" />
              <circle cx="1" cy="1" r="1" transform="translate(11 5)" />
              <circle cx="1" cy="1" r="1" transform="translate(5 5)" />
            </g>
          </svg>
        </div>
      </Tooltip>

      <div class="header_logo"></div>

      <div class="header_title">Tiền lương</div>
    </div>

    <!-- Header Right -->
    <div class="header_right">
      <Searchbar class="header_search" placeholder="Tìm kiếm..." width="300px" />

      <div class="header_actions">
        <!-- 1. Thiết lập (Bỏ màu cứng stroke/fill trong path) -->
        <Tooltip text="Thiết lập">
          <div class="header_item" title="Thiết lập">
            <svg width="20" height="20" viewBox="0 0 24 24">
              <g id="Group_58029" data-name="Group 58029">
                <path
                  data-name="Path 85802"
                  d="M10.325,4.317a1.724,1.724,0,0,1,3.35,0,1.724,1.724,0,0,0,2.573,1.066,1.725,1.725,0,0,1,2.37,2.37,1.724,1.724,0,0,0,1.065,2.572,1.724,1.724,0,0,1,0,3.35,1.724,1.724,0,0,0-1.066,2.573,1.725,1.725,0,0,1-2.37,2.37,1.724,1.724,0,0,0-2.572,1.065,1.724,1.724,0,0,1-3.35,0,1.724,1.724,0,0,0-2.573-1.066,1.725,1.725,0,0,1-2.37-2.37,1.724,1.724,0,0,0-1.065-2.572,1.724,1.724,0,0,1,0-3.35A1.724,1.724,0,0,0,5.383,7.752a1.725,1.725,0,0,1,2.37-2.37A1.723,1.723,0,0,0,10.325,4.317Z"
                />
                <path data-name="Path 85803" d="M12,15a3,3,0,1,0-3-3A3,3,0,0,0,12,15Z" />
              </g>
            </svg>
          </div>
        </Tooltip>

        <!-- 2. AI Agent (Standardized container) -->
        <Tooltip text="AI Trợ lý">
          <div class="header_item" title="AI Trợ lý">
            <div class="header_item_ai_icon"></div>
          </div>
        </Tooltip>

        <!-- 3. Tính năng mới -->
        <Tooltip text="Tính năng mới">
          <div class="header_item" title="Tính năng mới">
            <svg width="20" height="20" viewBox="0 0 24 24">
              <g id="Group_84810" data-name="Group 84810">
                <path
                  d="M18,8a3,3,0,0,1,0,6M10,8V19a1,1,0,0,1-1,1H8a1,1,0,0,1-1-1V14m5-6,4.524-3.77A.9.9,0,0,1,18,4.922V17.078a.9.9,0,0,1-1.476.692L12,14H4a1,1,0,0,1-1-1V9A1,1,0,0,1,4,8Z"
                />
              </g>
            </svg>
          </div>
        </Tooltip>

        <!-- 4. Tin nhắn -->
        <Tooltip text="Tin nhắn">
          <div class="header_item chat-svg" title="Tin nhắn">
            <svg
              width="20"
              height="20"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
              class="chat-svg"
            >
              <path
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M12 3C7.02944 3 3 7.02945 3 12C3 13.7499 3.49992 15.3827 4.36396 16.7645L3 21L7.23545 19.636C8.6173 20.5001 10.2501 21 12 21C16.9706 21 21 16.9706 21 12C21 7.02945 16.9706 3 12 3ZM12 13.0988C12.6068 13.0988 13.0986 12.6069 13.0986 12.0001C13.0986 11.3934 12.6068 10.9015 12 10.9015C11.3932 10.9015 10.9014 11.3934 10.9014 12.0001C10.9014 12.6069 11.3932 13.0988 12 13.0988ZM16.3945 13.0988C17.0013 13.0988 17.4932 12.6069 17.4932 12.0001C17.4932 11.3934 17.0013 10.9015 16.3945 10.9015C15.7878 10.9015 15.2959 11.3934 15.2959 12.0001C15.2959 12.6069 15.7878 13.0988 16.3945 13.0988ZM7.60547 13.0988C8.21222 13.0988 8.7041 12.6069 8.7041 12.0001C8.7041 11.3934 8.21222 10.9015 7.60547 10.9015C6.99872 10.9015 6.50684 11.3934 6.50684 12.0001C6.50684 12.6069 6.99872 13.0988 7.60547 13.0988Z"
                fill="white"
              />
            </svg>
          </div>
        </Tooltip>

        <!-- 5. Chuông thông báo -->
        <Tooltip text="Thông báo">
          <div class="header_item" title="Thông báo">
            <svg width="20" height="20" viewBox="0 0 24 24" class="bell-svg">
              <path
                d="M18,11.6V9.3a6.184,6.184,0,0,0-5.842-6.276A6,6,0,0,0,6,9.024V11.6c0,2.828-2.571,3.514-2.571,5.142C3.429,18.2,6.771,19.31,12,19.31s8.571-1.114,8.571-2.572C20.571,15.11,18,14.424,18,11.6Z"
              />
              <path
                d="M12,21.228q-1.25,0-2.5-.087a2.618,2.618,0,0,0,5,0Q13.325,21.226,12,21.228Z"
              />
            </svg>
          </div>
        </Tooltip>

        <!-- 6. Trợ giúp -->
        <Tooltip text="Hỏi đáp">
          <div class="header_item" title="Hỏi đáp">
            <svg width="20" height="20" viewBox="0 0 24 24" class="help-svg">
              <path d="M9.4,9.315a2.685,2.685,0,0,1,5.218.9C14.613,12.005,12,12.9,12,12.9v.9" />
              <path d="M12,21a9,9,0,1,0-9-9A9,9,0,0,0,12,21Z" />
              <path d="M12,18a1,1,0,1,0-1-1A1,1,0,0,0,12,18Z" class="dot-path" />
            </svg>
          </div>
        </Tooltip>

        <!-- 7. Tính năng khác -->
        <Tooltip text="Tính năng khác">
          <div class="header_item" title="Tính năng khác">
            <svg width="20" height="20" viewBox="0 0 24 24">
              <g id="Group_56632" data-name="Group 56632" transform="translate(-132 0)">
                <path data-name="Path 85796" d="M144,21a9,9,0,1,0-9-9A9,9,0,0,0,144,21Z" />
                <path data-name="Path 85797" d="M144,12v.01" />
                <path data-name="Path 85798" d="M140,12v.01" />
                <path data-name="Path 85799" d="M148,12v.01" />
              </g>
            </svg>
          </div>
        </Tooltip>

        <!-- 8. Kiến thức hữu ích -->
        <Tooltip text="Kiến thức hữu ích">
          <div class="header_item is-yellow" title="Kiến thức hữu ích">
            <svg
              width="20"
              height="20"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M3 10V21"
                stroke="#FFAA2B"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <path
                d="M3 10C3.06667 9.66667 3.56 9 5 9"
                stroke="#FFAA2B"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <path
                d="M21 10C20.9333 9.66667 20.44 9 19 9"
                stroke="#FFAA2B"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <path
                d="M21 10V21"
                stroke="#FFAA2B"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <path
                d="M3 21C4.36817 20.21 5.92017 19.7942 7.5 19.7942C9.07983 19.7942 10.6318 20.21 12 21C13.3682 20.21 14.9202 19.7942 16.5 19.7942C18.0798 19.7942 19.6318 20.21 21 21"
                stroke="#FFAA2B"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <path
                d="M9.6 12.0714C8.92838 11.5767 8.43227 10.887 8.18195 10.0999C7.93163 9.31291 7.93978 8.46847 8.20526 7.68625C8.47075 6.90402 8.9801 6.22366 9.66116 5.74155C10.3422 5.25943 11.1605 5 12 5C12.8395 5 13.6578 5.25943 14.3388 5.74155C15.0199 6.22366 15.5293 6.90402 15.7947 7.68625C16.0602 8.46847 16.0684 9.31291 15.8181 10.0999C15.5677 10.887 15.0716 11.5767 14.4 12.0714C14.0877 12.3751 13.8525 12.7467 13.7135 13.1562C13.5745 13.5657 13.5356 14.0016 13.6 14.4286C13.6 14.8453 13.4314 15.245 13.1314 15.5397C12.8313 15.8344 12.4243 16 12 16C11.5757 16 11.1687 15.8344 10.8686 15.5397C10.5686 15.245 10.4 14.8453 10.4 14.4286C10.4644 14.0016 10.4255 13.5657 10.2865 13.1562C10.1475 12.7467 9.91235 12.3751 9.6 12.0714Z"
                stroke="#FFAA2B"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <path
                d="M10.1602 12.8572H13.8402"
                stroke="#FFAA2B"
                stroke-width="1.5"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <path
                d="M12 2L12 1"
                stroke="#FFAA2B"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <path
                d="M7 4L6.04499 3.70344"
                stroke="#FFAA2B"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <path
                d="M16.9998 4L17.9548 3.70344"
                stroke="#FFAA2B"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
            </svg>
          </div>
        </Tooltip>

        <!-- User Avatar (Item thứ 9) -->
        <div class="header_avatar">
          <img
            src="https://amisplatform.misacdn.net/APIS/PlatformAPI/api/Avatar/77b8feac-dfe5-4b47-b1ff-ac9a2eaced5a/LT0YJBOP.jpg"
            alt="Avatar"
          />
        </div>
      </div>
    </div>
  </header>
</template>

<script setup>
import Searchbar from '@/components/controls/inputs/Searchbar.vue'
import Tooltip from '@/components/base/Tooltip.vue'
</script>

<style scoped>
.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: fixed;
  top: 0;
  right: 0;
  left: 0;
  height: var(--header-height);
  background-color: var(--color-primary);
  z-index: var(--z-index-overlay);
  color: #ffffff;
}

.header_left {
  display: flex;
  align-items: center;
  padding-right: 24px;
}

.header_apps {
  width: 36px;
  height: 36px;
  margin: 0 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.header_apps:hover {
  background-color: #0000001a;
}

.header_apps svg * {
  stroke: #fff !important;
  fill: none;
}

.header_logo {
  background-image: url('https://testcdnamisapp.misacdn.net/apps/payroll/static/logos/icon-payroll-logo.svg?v=');
  width: 32px;
  height: 32px;
  background-repeat: no-repeat;
  background-size: contain;
}

.header_title {
  font-weight: 700;
  font-size: 18px;
  margin-left: 12px;
  color: #fff;
}

.header_right {
  display: flex;
  align-items: center;
  margin-right: 16px;
  gap: 16px;
}

.header_search {
  width: 300px;
  flex-shrink: 0;
}

/* Customize Searchbar for Header primary background */
.header_search :deep(.input_form),
.header_search :deep(.input_form:hover),
.header_search :deep(.input_form:focus) {
  background-color: #fff3 !important; /* Ghi đè màu trắng mặc định để lộ nền xanh bên dưới */
  border: none;
  color: #fff;
  outline: none;
}

.header_search :deep(.m-input-icon) {
  color: #fff;
}

.header_search :deep(.input_form::placeholder) {
  color: rgba(255, 255, 255, 0.7);
}

.header_actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

.header_item {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.header_item:hover {
  background-color: #0000001a;
}

.header_item svg {
  width: 24px;
  height: 24px;
}

.header_item svg * {
  stroke: #fff !important;
  fill: none;
  stroke-linecap: round;
  stroke-linejoin: round;
  stroke-width: 2;
}

/* Đảm bảo các thành phần của chat icon không bị stroke đè */
.header_item svg.chat-svg * {
  stroke: none !important;
}

/* Bong bóng chat luôn màu trắng */
.header_item svg.chat-svg path:first-child {
  fill: #fff !important;
}

/* 3 chấm trong icon tin nhắn phải trong suốt */
.header_item svg.chat-svg path:not(:first-child) {
  display: none;
}

/* Icon chuông thông báo */
.header_item svg.bell-svg path:first-child {
  /* Phần chuông bên ngoài */
  fill: none !important;
  stroke: #fff !important;
}
.header_item svg.bell-svg path:last-child {
  /* Phần cục ở giữa */
  fill: #fff !important;
  stroke: none !important;
}

.header_item.is-filled svg path {
  fill: #fff !important;
  stroke: #fff !important;
}

/* Xử lý riêng cho chấm tròn của icon trợ giúp để nó hiển thị dạng fill thay vì outline */
.header_item svg.help-svg .dot-path {
  fill: #fff !important;
  stroke: none !important;
}

.header_item.is-yellow svg * {
  stroke: #ffaa2b !important;
  fill: none !important;
}

.header_item_ai_icon {
  width: 25px;
  height: 25px;
  background-repeat: no-repeat;
  background-size: contain;
  background-image: url('https://testcdnamisapp.misacdn.net/apps/payroll/static/img/payroll_agent-d_2srbQV.png');
}

.header_avatar {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.header_avatar:hover {
  background-color: #0000001a;
}

.header_avatar img {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  object-fit: cover;
}
</style>
