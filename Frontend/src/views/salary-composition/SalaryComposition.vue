<template>
  <!-- Nếu là route con (danh mục hệ thống), hiển thị router-view -->
  <router-view v-if="$route.path.includes('system-category')" />

  <SalaryCompositionForm
    v-else-if="isFormVisible"
    :composition-id="editId"
    :duplicate-id="duplicateId"
    @close="isFormVisible = false"
    @save="fetchData"
    @duplicate="handleDuplicateFromForm"
    @error-notification="handleFormNotification"
  />

  <template v-else>
    <ThePageContainer title="Thành phần lương">
      <template #header-right>
        <ButtonGroup>
          <!--btn danh mục hệ thống-->
          <Button
            height="32px"
            variant="secondary"
            padding="0 12px"
            @click="$router.push('/salary-composition/system-category')"
          >
            <template #icon>
              <svg
                width="20"
                height="20"
                viewBox="82.5 102.5 15 15"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <g id="Icon Placeholder_105">
                  <path
                    id="Vector_95"
                    d="M 85.8333,105.833 H 85 c -0.442,0 -0.866,0.176 -1.1785,0.488 -0.3126,0.313 -0.4882,0.737 -0.4882,1.179 v 7.5 c 0,0.442 0.1756,0.866 0.4882,1.179 0.3125,0.312 0.7365,0.488 1.1785,0.488 h 7.5 c 0.442,0 0.8659,-0.176 1.1785,-0.488 0.3126,-0.313 0.4881,-0.737 0.4881,-1.179 v -0.833 m -0.8333,-10 2.5,2.5 m 1.1542,-1.18 c 0.3282,-0.328 0.5126,-0.773 0.5126,-1.237 0,-0.464 -0.1844,-0.909 -0.5126,-1.238 -0.3282,-0.328 -0.7734,-0.512 -1.2375,-0.512 -0.4642,0 -0.9093,0.184 -1.2375,0.512 L 87.5,110 v 2.5 H 90 Z"
                    stroke="var(--color-text-secondary)"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </g>
              </svg>
            </template>
            Danh mục của hệ thống
          </Button>

          <!-- Nút Thêm mới (Split Button) -->
          <Button split :dropdown-items="addDropdownItems" @click="openModal" height="32px">
            <template #icon><div class="content_header_btn_add_main_icon"></div></template>
            Thêm
          </Button>
        </ButtonGroup>
      </template>

      <!--action bar-->
      <div class="content_body_actions">
        <div class="content_body_actions_left">
          <!-- Search box luôn hiển thị -->
          <div class="search_box">
            <Searchbar v-model="searchQuery" placeholder="Tìm kiếm" width="300px" />
          </div>

          <!-- Khối tác vụ hàng loạt khi có dòng được chọn -->
          <template v-if="selectedIds.length > 0">
            <span class="selection-info">
              Đã chọn
              <strong style="font-weight: 700; margin-left: 4px">{{ selectedIds.length }}</strong>
            </span>
            <Button
              variant="text"
              color="var(--border-control-hover)"
              height="32px"
              @click="selectedIds = []"
              >Bỏ chọn</Button
            >
            <ButtonGroup>
              <!-- Nút Ngừng theo dõi -->
              <Button
                v-if="showBulkStopTrackingButton"
                variant="outline"
                color="var(--color-warning)"
                hover-bg-color="#fef0c7"
                height="32px"
                @click="handleBulkUpdateStatus(1)"
              >
                <template #icon>
                  <svg
                    width="16"
                    height="16"
                    viewBox="182.5 42.5 15 15"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <path
                      d="M187.5 50H192.5M182.5 50C182.5 50.9849 182.694 51.9602 183.071 52.8701C183.448 53.7801 184 54.6069 184.697 55.3033C185.393 55.9997 186.22 56.5522 187.13 56.9291C188.04 57.306 189.015 57.5 190 57.5C190.985 57.5 191.96 57.306 192.87 56.9291C193.78 56.5522 194.607 55.9997 195.303 55.3033C196 54.6069 196.552 53.7801 196.929 52.8701C197.306 51.9602 197.5 50.9849 197.5 50C197.5 49.0151 197.306 48.0398 196.929 47.1299C196.552 46.2199 196 45.3931 195.303 44.6967C194.607 44.0003 193.78 43.4478 192.87 43.0709C191.96 42.694 190.985 42.5 190 42.5C189.015 42.5 188.04 42.694 187.13 43.0709C186.22 43.4478 185.393 44.0003 184.697 44.6967C184 45.3931 183.448 46.2199 183.071 47.1299C182.694 48.0398 182.5 49.0151 182.5 50Z"
                      stroke="currentColor"
                      stroke-width="1.5"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                    />
                  </svg>
                </template>
                Ngừng theo dõi
              </Button>

              <!-- Nút Đang theo dõi -->
              <Button
                v-if="showBulkStartTrackingButton"
                variant="outline"
                color="var(--color-primary)"
                hover-bg-color="#a8d9c8"
                height="32px"
                @click="handleBulkUpdateStatus(0)"
              >
                <template #icon>
                  <svg
                    width="16"
                    height="16"
                    viewBox="142.5 42.5 15 15"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <g id="Icon Placeholder_48">
                      <path
                        id="Vector_43"
                        d="M147.5 50L149.167 51.6667L152.5 48.3333M142.5 50C142.5 50.9849 142.694 51.9602 143.071 52.8701C143.448 53.7801 144 54.6069 144.697 55.3033C145.393 55.9997 146.22 56.5522 147.13 56.9291C148.04 57.306 149.015 57.5 150 57.5C150.985 57.5 151.96 57.306 152.87 56.9291C153.78 56.5522 154.607 55.9997 155.303 55.3033C156 54.6069 156.552 53.7801 156.929 52.8701C157.306 51.9602 157.5 50.9849 157.5 50C157.5 49.0151 157.306 48.0398 156.929 47.1299C156.552 46.2199 156 45.3931 155.303 44.6967C154.607 44.0003 153.78 43.4478 152.87 43.0709C151.96 42.694 150.985 42.5 150 42.5C149.015 42.5 148.04 42.694 147.13 43.0709C146.22 43.4478 145.393 44.0003 144.697 44.6967C144 45.3931 143.448 46.2199 143.071 47.1299C142.694 48.0398 142.5 49.0151 142.5 50Z"
                        stroke="currentColor"
                        stroke-width="1.5"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                    </g>
                  </svg>
                </template>
                Đang theo dõi
              </Button>

              <!-- Nút Xóa -->
              <Button
                variant="outline"
                color="var(--color-error)"
                hover-bg-color="#fee4e2"
                height="32px"
                @click="handleBulkDelete"
              >
                <template #icon>
                  <TrashIcon />
                </template>
                Xóa
              </Button>
            </ButtonGroup>
          </template>

          <!-- Khối select lọc dữ liệu, chỉ hiển thị khi không có dòng nào được chọn -->
          <template v-else>
            <SelectLabel
              v-model="selectedStatus"
              label="Trạng thái"
              :options="statusOptions"
              option-label="label"
              option-value="value"
              width="auto"
              dropdown-width="160px"
            />

            <SelectTree
              v-model="selectedOrganizations"
              placeholder="Tất cả đơn vị"
              :options="organizationOptions"
              width="350px"
              node-key="OrganizationId"
              label-key="OrganizationName"
              children-key="Children"
              dropdown-width="350px"
              style="width: 350px"
              :max-tags="1"
            />
          </template>
        </div>

        <!--action btns bên phải-->
        <template v-if="selectedIds.length === 0">
          <ButtonGroup>
            <!--Nút bộ lọc-->
            <ButtonIcon
              variant="outline"
              title="Bộ lọc"
              @click="isFilterDrawerOpen = !isFilterDrawerOpen"
              tooltip
            >
              <svg
                width="20"
                height="20"
                viewBox="20 0 20 20"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <g id="Icon Placeholder_2">
                  <path
                    id="Vector_2"
                    d="m 23.3333,3.33333 h 13.3334 v 1.81 c -10e-5,0.44199 -0.1758,0.86585 -0.4884,1.17834 L 32.5,10 v 5.8333 L 27.5,17.5 V 10.4167 L 23.7667,6.31 C 23.4879,6.00327 23.3334,5.60366 23.3333,5.18917 Z"
                    stroke="currentColor"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </g>
              </svg>
            </ButtonIcon>

            <!--Nút thiết lập cột-->
            <ButtonIcon
              variant="outline"
              title="Thiết lập bảng"
              @click="handleOpenTableSetting"
              padding="6px"
              tooltip
            >
              <svg
                width="20"
                height="20"
                viewBox="320 80 20 20"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <g id="Icon Placeholder_97">
                  <g id="Vector_88">
                    <path
                      d="m 328.604,83.5975 c 0.355,-1.4633 2.437,-1.4633 2.792,0 0.053,0.2198 0.158,0.424 0.305,0.5958 0.147,0.1719 0.332,0.3066 0.541,0.3932 0.209,0.0865 0.436,0.1225 0.661,0.1051 0.226,-0.0175 0.444,-0.088 0.637,-0.2058 1.286,-0.7833 2.758,0.6884 1.975,1.975 -0.118,0.1931 -0.188,0.4111 -0.205,0.6365 -0.018,0.2254 0.018,0.4517 0.105,0.6605 0.086,0.2089 0.221,0.3943 0.392,0.5414 0.172,0.1471 0.376,0.2516 0.596,0.305 1.463,0.355 1.463,2.4366 0,2.7916 -0.22,0.0533 -0.424,0.1577 -0.596,0.3048 -0.172,0.1471 -0.307,0.3326 -0.393,0.5416 -0.087,0.209 -0.123,0.4354 -0.106,0.661 0.018,0.2255 0.088,0.4437 0.206,0.6368 0.784,1.2858 -0.688,2.7583 -1.975,1.975 -0.193,-0.1176 -0.411,-0.188 -0.636,-0.2054 -0.226,-0.0174 -0.452,0.0185 -0.661,0.105 -0.209,0.0865 -0.394,0.221 -0.541,0.3927 -0.147,0.1716 -0.252,0.3756 -0.305,0.5952 -0.355,1.4633 -2.437,1.4633 -2.792,0 -0.053,-0.2198 -0.158,-0.424 -0.305,-0.5958 -0.147,-0.1719 -0.332,-0.3066 -0.541,-0.3932 -0.209,-0.0865 -0.436,-0.1225 -0.661,-0.1051 -0.226,0.0175 -0.444,0.088 -0.637,0.2058 -1.286,0.7833 -2.758,-0.6884 -1.975,-1.975 0.118,-0.1931 0.188,-0.4111 0.205,-0.6365 0.018,-0.2254 -0.018,-0.4517 -0.105,-0.6605 -0.086,-0.2089 -0.221,-0.3943 -0.392,-0.5414 -0.172,-0.1471 -0.376,-0.2516 -0.595,-0.305 -1.464,-0.355 -1.464,-2.4366 0,-2.7916 0.219,-0.0533 0.423,-0.1577 0.595,-0.3048 0.172,-0.1471 0.307,-0.3326 0.393,-0.5416 0.087,-0.209 0.123,-0.4354 0.106,-0.661 -0.018,-0.2255 -0.088,-0.4437 -0.206,-0.6368 -0.784,-1.2858 0.688,-2.7583 1.975,-1.975 0.833,0.5067 1.913,0.0583 2.143,-0.8875 z"
                      stroke="#717680"
                      stroke-width="1.5"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                    />
                    <path
                      d="m 327.5,90 c 0,0.663 0.263,1.2989 0.732,1.7678 0.469,0.4688 1.105,0.7322 1.768,0.7322 0.663,0 1.299,-0.2634 1.768,-0.7322 0.469,-0.4689 0.732,-1.1048 0.732,-1.7678 0,-0.663 -0.263,-1.2989 -0.732,-1.7678 C 331.299,87.7634 330.663,87.5 330,87.5 c -0.663,0 -1.299,0.2634 -1.768,0.7322 C 327.763,88.7011 327.5,89.337 327.5,90 Z"
                      stroke="#717680"
                      stroke-width="1.5"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                    />
                  </g>
                </g>
              </svg>
            </ButtonIcon>
          </ButtonGroup>
        </template>
      </div>

      <!--content table-->
      <div class="table-container">
        <Table
          :headers="visibleColumns"
          :data="salaryCompositions"
          :selectable="true"
          row-key="ScId"
          v-model:selection="selectedIds"
          @header-click="handleHeaderClick"
          @row-click="handleEdit"
        >
          <template #ScCode="{ data }">
            <span :title="data.ScCode">{{ data.ScCode ?? '-' }}</span>
          </template>
          <template #ScName="{ data }">
            <span :title="data.ScName">{{ data.ScName ?? '-' }}</span>
          </template>
          <template #OrganizationNames="{ data }">
            <span :title="data.AppliedOrganizations?.map((o) => o.OrganizationName).join(', ')">
              {{
                data.AppliedOrganizations?.length > 0
                  ? data.AppliedOrganizations.map((o) => o.OrganizationName).join(', ')
                  : rootOrganizationName
              }}
            </span>
          </template>
          <template #ScType="{ data }">
            <span :title="data.ScType !== null ? getTypeName(data.ScType) : ''">
              {{ data.ScType !== null ? getTypeName(data.ScType) : '-' }}
            </span>
          </template>
          <template #ScNature="{ data }">
            <span :title="data.ScNature !== null ? getNatureText(data.ScNature) : ''">
              {{ data.ScNature !== null ? getNatureText(data.ScNature) : '-' }}
            </span>
          </template>
          <template #TaxStatus="{ data }">
            <span :title="data.TaxStatus !== null ? getTaxStatusText(data.TaxStatus) : ''">
              {{ data.TaxStatus !== null ? getTaxStatusText(data.TaxStatus) : '-' }}
            </span>
          </template>
          <template #IsTaxDeductible="{ data }">
            <span
              :title="data.IsTaxDeductible !== null ? (data.IsTaxDeductible ? 'Có' : 'Không') : ''"
            >
              {{ data.IsTaxDeductible !== null ? (data.IsTaxDeductible ? 'Có' : 'Không') : '-' }}
            </span>
          </template>
          <template #LimitExpression="{ data }">
            <span :title="data.LimitExpression">
              {{ data.LimitExpression ?? '-' }}
            </span>
          </template>
          <template #ValueType="{ data }">
            <span :title="data.ValueType !== null ? getValueTypeText(data.ValueType) : ''">
              {{ data.ValueType !== null ? getValueTypeText(data.ValueType) : '-' }}
            </span>
          </template>
          <template #FormulaExpression="{ data }">
            <span v-if="data.CalculationMethod === 0" title="Tự động cộng tổng">
              Tự động cộng tổng
            </span>
            <span
              v-else
              :title="data.FormulaExpression || '-'"
              class="formula-highlight-cell"
              v-html="highlightFormula(data.FormulaExpression) || '-'"
            ></span>
          </template>
          <template #ScStatus="{ data }">
            <div
              class="status-tag"
              :class="{
                'status-tag--active': data.ScStatus === 0,
                'status-tag--inactive': data.ScStatus === 1,
              }"
            >
              <div class="status-dot"></div>
              <span>{{ data.ScStatus === 0 ? 'Đang theo dõi' : 'Ngừng theo dõi' }}</span>
            </div>
          </template>
          <template #Description="{ data }">
            <span :title="data.Description">{{ data.Description ?? '-' }}</span>
          </template>
          <template #ScSource="{ data }">
            <span :title="data.ScSource !== null ? getSourceText(data.ScSource) : ''">
              {{ data.ScSource !== null ? getSourceText(data.ScSource) : '-' }}
            </span>
          </template>
          <template #IsDisplayedOnPayroll="{ data }">
            <span
              :title="
                data.IsDisplayedOnPayroll !== null
                  ? getIsDisplayedOnPayrollText(data.IsDisplayedOnPayroll)
                  : ''
              "
            >
              {{
                data.IsDisplayedOnPayroll !== null
                  ? getIsDisplayedOnPayrollText(data.IsDisplayedOnPayroll)
                  : '-'
              }}
            </span>
          </template>

          <!-- Body slot cho các thao tác trên dòng -->
          <template #row-actions="{ data }">
            <ButtonGroup gap="12px">
              <!-- Ngừng/Tiếp tục theo dõi -->
              <ButtonIcon
                variant="outline"
                width="28px"
                height="28px"
                min-width="28px"
                padding="6px"
                icon-size="16"
                :color="data.ScStatus === 0 ? 'var(--color-warning)' : 'var(--color-primary)'"
                :title="data.ScStatus === 0 ? 'Ngừng theo dõi' : 'Tiếp tục theo dõi'"
                @click.stop="handleToggleStatus(data)"
                tooltip
              >
                <svg
                  v-if="data.ScStatus === 0"
                  width="16"
                  height="16"
                  viewBox="182.5 42.5 15 15"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <g id="Icon Placeholder_50">
                    <path
                      id="Vector_45"
                      d="M187.5 50H192.5M182.5 50C182.5 50.9849 182.694 51.9602 183.071 52.8701C183.448 53.7801 184 54.6069 184.697 55.3033C185.393 55.9997 186.22 56.5522 187.13 56.9291C188.04 57.306 189.015 57.5 190 57.5C190.985 57.5 191.96 57.306 192.87 56.9291C193.78 56.5522 194.607 55.9997 195.303 55.3033C196 54.6069 196.552 53.7801 196.929 52.8701C197.306 51.9602 197.5 50.9849 197.5 50C197.5 49.0151 197.306 48.0398 196.929 47.1299C196.552 46.2199 196 45.3931 195.303 44.6967C194.607 44.0003 193.78 43.4478 192.87 43.0709C191.96 42.694 190.985 42.5 190 42.5C189.015 42.5 188.04 42.694 187.13 43.0709C186.22 43.4478 185.393 44.0003 184.697 44.6967C184 45.3931 183.448 46.2199 183.071 47.1299C182.694 48.0398 182.5 49.0151 182.5 50Z"
                      stroke="currentColor"
                      stroke-width="1.5"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                    />
                  </g>
                </svg>
                <svg
                  v-else
                  width="16"
                  height="16"
                  viewBox="142.5 42.5 15 15"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <g id="Icon Placeholder_48">
                    <path
                      id="Vector_43"
                      d="M147.5 50L149.167 51.6667L152.5 48.3333M142.5 50C142.5 50.9849 142.694 51.9602 143.071 52.8701C143.448 53.7801 144 54.6069 144.697 55.3033C145.393 55.9997 146.22 56.5522 147.13 56.9291C148.04 57.306 149.015 57.5 150 57.5C150.985 57.5 151.96 57.306 152.87 56.9291C153.78 56.5522 154.607 55.9997 155.303 55.3033C156 54.6069 156.552 53.7801 156.929 52.8701C157.306 51.9602 157.5 50.9849 157.5 50C157.5 49.0151 157.306 48.0398 156.929 47.1299C156.552 46.2199 156 45.3931 155.303 44.6967C154.607 44.0003 153.78 43.4478 152.87 43.0709C151.96 42.694 150.985 42.5 150 42.5C149.015 42.5 148.04 42.694 147.13 43.0709C146.22 43.4478 145.393 44.0003 144.697 44.6967C144 45.3931 143.448 46.2199 143.071 47.1299C142.694 48.0398 142.5 49.0151 142.5 50Z"
                      stroke="currentColor"
                      stroke-width="1.5"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                    />
                  </g>
                </svg>
              </ButtonIcon>

              <!-- Nhân bản -->
              <ButtonIcon
                variant="outline"
                width="28px"
                height="28px"
                min-width="28px"
                padding="6px"
                icon-size="16"
                color="var(--color-icon)"
                title="Nhân bản"
                @click.stop="handleDuplicate(data)"
                tooltip
              >
                <CopyIcon />
              </ButtonIcon>

              <!-- Sửa -->
              <ButtonIcon
                variant="outline"
                width="28px"
                height="28px"
                min-width="28px"
                padding="6px"
                icon-size="16"
                color="var(--color-icon)"
                title="Sửa"
                @click.stop="handleEdit(data)"
                tooltip
              >
                <PencilIcon />
              </ButtonIcon>

              <!-- Xóa -->
              <ButtonIcon
                variant="outline"
                width="28px"
                height="28px"
                min-width="28px"
                padding="6px"
                icon-size="16"
                color="var(--color-error)"
                title="Xóa"
                @click.stop="handleDelete(data)"
                tooltip
              >
                <TrashIcon />
              </ButtonIcon>
            </ButtonGroup>
          </template>
        </Table>
      </div>

      <!--pagination-->
      <Pagination
        v-model:current-page="currentPage"
        v-model:page-size="pageSize"
        :total-records="totalRecords"
      />

      <!-- Menu dropdown cho header table (Sắp xếp & Ghim) -->
      <ColumnMenu
        v-if="isColumnMenuOpen"
        v-model:visible="isColumnMenuOpen"
        :column="activeFilterColumn"
        :sort-order="currentSort.key === activeFilterColumn?.key ? currentSort.order : null"
        :is-pinned="activeFilterColumn?.pinned === 'left'"
        :top="columnMenuPosition.top"
        :left="columnMenuPosition.left"
        @sort="handleSort"
        @pin="handleTogglePin"
      />

      <template #sidebar>
        <FilterDrawer
          v-show="isFilterDrawerOpen"
          :applied-filters="appliedFilters"
          :columns="filterDrawerColumns"
          @close="isFilterDrawerOpen = false"
          @apply="handleFilterApply"
        />
      </template>
    </ThePageContainer>

    <!-- Modal thiết lập cột -->
    <TableSetting
      v-if="isTableSettingVisible"
      :visible="isTableSettingVisible"
      :columns="columnsConfig"
      :top="tableSettingPos.top"
      :right="tableSettingPos.right"
      @save="handleColumnSettingsSave"
      @reset="handleColumnSettingsReset"
      @update:visible="isTableSettingVisible = $event"
    />

    <!-- Modal xác nhận xóa -->
    <ConfirmationModal
      v-model:visible="isDeleteModalVisible"
      :message="deleteConfirmMessage"
      confirm-button-text="Xóa"
      confirm-button-variant="danger"
      cancel-button-text="Hủy"
      @confirm="confirmDelete"
    />

    <!-- Modal xác nhận cập nhật trạng thái hàng loạt -->
    <ConfirmationModal
      v-model:visible="isStatusConfirmVisible"
      :message="statusConfirmMessage"
      @confirm="confirmBulkUpdateStatus"
    />
  </template>
  <!-- Modal thông báo không thể xóa bản ghi hệ thống -->
  <NotificationModal
    v-model:visible="isNotificationModalVisible"
    title="Thông báo"
    :message="notificationMessage"
  />

  <!-- Modal thêm từ hệ thống -->
  <AddFromSystem
    v-if="isAddFromSystemVisible"
    v-model:visible="isAddFromSystemVisible"
    @confirm="handleConfirmAddFromSystem"
  />
</template>

<style scoped>
/* Icon đặc thù của MISA cho nút thêm */
.content_header_btn_add_main_icon {
  background-image: url('https://amisplatform.misacdn.net/apps/recruit2/assets/images/ICON.svg');
  width: 20px;
  height: 20px;
  background-position: -20px -656px;
}

.content_body_actions {
  background-color: white;
  border-radius: 8px 8px 0 0;
  padding: 12px 16px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.selection-info {
  font-size: 13px;
  color: var(--color-text-main);
}

.content_body_actions_left {
  display: flex;
  align-items: center;
  gap: 8px;
}

.search_box {
  width: 300px;
}

.table-container {
  width: 100%;
  background-color: white;
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0; /* Quan trọng: cho phép flex item thu nhỏ lại để kích hoạt scrollbar bên trong */
}

.status-tag {
  height: 24px;
  border-radius: 8px;
  border: 1px solid;
  padding: 2px 8px;
  display: flex;
  align-items: center;
  gap: 6px;
  width: fit-content;
  font-size: 13px;
  font-weight: 500;
}

.status-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  flex-shrink: 0;
}

/* Styles for "Đang theo dõi" (Active) */
.status-tag--active {
  color: var(--color-primary);
  border-color: #75e0ac; /* Màu border cụ thể cho trạng thái active */
  background-color: #edfcf4; /* Màu nền cụ thể cho trạng thái active */
}

.status-tag--active .status-dot {
  background-color: var(--color-primary);
}

/* Styles for "Ngừng theo dõi" (Inactive) */
.status-tag--inactive {
  color: #dc6803;
  border-color: #fedf89; /* Màu border cụ thể cho trạng thái inactive */
  background-color: #fffaeb; /* Màu nền cụ thể cho trạng thái inactive */
}
.status-tag--inactive .status-dot {
  background-color: var(--color-warning);
}

/* Style cho highlight công thức trong table */
.formula-highlight-cell :deep(span) {
  font-family: 'Inter', sans-serif !important;
  font-size: 13px;
}

/* Responsive Adjustments */
@media (max-width: 1024px) {
  .content_body_actions {
    flex-direction: column;
    align-items: stretch;
    gap: 12px;
  }

  .content_body_actions_search {
    width: 100%;
  }

  .content_body_actions_btns {
    justify-content: flex-end;
  }

  .content_header_btn_delete_text {
    max-width: 150px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
}

/*end of style content*/
</style>

<script setup>
import ThePageContainer from '@/layout/ThePageContainer.vue'
import Button from '@/components/controls/buttons/Button.vue'
import ButtonGroup from '@/components/controls/buttons/ButtonGroup.vue'
import ButtonIcon from '@/components/controls/buttons/ButtonIcon.vue'
import Searchbar from '@/components/controls/inputs/Searchbar.vue'
import SelectLabel from '@/components/controls/selects/SelectLabel.vue'
import SelectTree from '@/components/controls/selects/SelectTree.vue'
import Table from '@/components/controls/tables/Table.vue'
import Pagination from '@/components/controls/pagination/Pagination.vue'
import ColumnMenu from '@/components/controls/tables/ColumnMenu.vue'
import FilterDrawer from '@/components/drawer/FilterDrawer.vue'
import TableSetting from '@/components/base/TableSetting.vue'
import ConfirmationModal from '@/components/base/ConfirmationModal.vue'
import NotificationModal from '@/components/base/NotificationModal.vue'
import AddFromSystem from './AddFromSystem.vue'
import SalaryCompositionForm from './SalaryCompositionForm.vue'

import CopyIcon from '@/components/icons/CopyIcon.vue'
import TrashIcon from '@/components/icons/TrashIcon.vue'
import PencilIcon from '@/components/icons/PencilIcon.vue'

import {
  getTypeName,
  getNatureText,
  getTaxStatusText,
  getValueTypeText,
  getSourceText,
  getIsDisplayedOnPayrollText,
} from '@/utils/salary-composition-helpers.js'

import useSalaryComposition from './useSalaryComposition.js'

const {
  salaryCompositions,
  totalRecords,
  isLoading,
  searchQuery,
  isFilterDrawerOpen,
  isTableSettingVisible,
  tableSettingPos,
  isColumnMenuOpen,
  activeFilterColumn,
  columnMenuPosition,
  currentSort,
  appliedFilters,
  isFormVisible,
  isDeleteModalVisible,
  isNotificationModalVisible,
  notificationMessage,
  editId,
  duplicateId,
  isAddFromSystemVisible,
  currentPage,
  pageSize,
  columnsConfig,
  visibleColumns,
  selectedIds,
  addDropdownItems,
  statusOptions,
  selectedStatus,
  selectedOrganizations,
  organizationOptions,
  rootOrganizationName,
  filterDrawerColumns,
  deleteConfirmMessage,
  showBulkStopTrackingButton,
  showBulkStartTrackingButton,
  isStatusConfirmVisible,
  statusConfirmMessage,
  openModal,
  handleEdit,
  handleDelete,
  handleConfirmAddFromSystem,
  confirmDelete,
  handleBulkUpdateStatus,
  confirmBulkUpdateStatus,
  handleBulkDelete,
  handleDuplicate,
  handleDuplicateFromForm,
  handleFormNotification,
  handleToggleStatus,
  fetchData,
  handleFilterApply,
  handleOpenTableSetting,
  handleColumnSettingsSave,
  handleColumnSettingsReset,
  handleHeaderClick,
  handleSort,
  handleTogglePin,
  highlightFormula,
} = useSalaryComposition()
</script>
