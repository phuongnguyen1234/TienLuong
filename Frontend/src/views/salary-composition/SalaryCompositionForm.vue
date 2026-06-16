<template>
  <!-- Modal xác nhận xóa -->
  <ConfirmationModal
    v-model:visible="isDeleteModalVisible"
    :message="`Bạn có chắc chắn muốn xóa thành phần lương <${form.sc_name}> không?`"
    confirm-button-text="Xóa"
    confirm-button-variant="danger"
    cancel-button-text="Hủy"
    @confirm="confirmDelete"
  />

  <!-- Modal xác nhận thay đổi chưa lưu -->
  <ConfirmationModal
    v-model:visible="isUnsavedChangesModalVisible"
    message="Nếu bạn thoát, các dữ liệu đang nhập liệu sẽ không được lưu lại."
    confirm-button-text="Thoát, không lưu"
    cancel-button-text="Ở lại"
    confirm-button-variant="primary"
    @confirm="confirmExit"
  />

  <ThePageContainer
    :title="formTitle"
    :can-go-back="true"
    @back="handleClose"
    class="m-salary-form-main"
    padding-bottom="0"
  >
    <!-- Header Right: Chứa các nút cho chế độ Xem và Sửa (isEditMode = true) -->
    <template #header-right>
      <template v-if="isEditMode">
        <ButtonGroup>
          <!-- Chế độ Xem (Khi đã có ID và chưa nhấn Sửa) -->
          <template v-if="isReadOnly">
            <Button variant="secondary" height="32px" @click="isReadOnly = false">
              <template #icon>
                <PencilIcon />
              </template>
              Sửa
            </Button>
          </template>

          <!-- Chế độ Sửa (Khi đã có ID và đang trong trạng thái Edit) -->
          <template v-else>
            <Button variant="secondary" height="32px" @click="handleCancelEdit"> Hủy bỏ </Button>
            <Button variant="primary" height="32px" @click="handleSave(false)"> Lưu </Button>
          </template>

          <!-- Nút tùy chọn luôn hiển thị ở chế độ Sửa/Xem của Record cũ -->
          <ButtonIcon
            variant="secondary"
            height="32px"
            :dropdown-items="moreActions"
            tooltip
            title="Chức năng khác"
          >
            <template #icon>
              <MoreIcon />
            </template>
          </ButtonIcon>
        </ButtonGroup>
      </template>
    </template>

    <!-- Footer: Chỉ hiển thị ở chế độ Thêm mới (!isEditMode) -->
    <template #footer>
      <div v-if="!isEditMode" class="m-form-footer">
        <ButtonGroup>
          <Button variant="secondary" height="32px" @click="handleCancelEdit"> Hủy bỏ </Button>
          <Button
            variant="outline"
            color="var(--color-primary)"
            height="32px"
            @click="handleSave(true)"
          >
            Lưu và thêm
          </Button>
          <Button variant="primary" height="32px" @click="handleSave(false)"> Lưu </Button>
        </ButtonGroup>
      </div>
    </template>

    <div class="form-scroll-content">
      <FormInputSection>
        <FormInputRow label="Tên thành phần" required :error="nameError" direction="horizontal">
          <Input
            v-model="form.sc_name"
            :error="!!nameError"
            ref="firstInputRef"
            :disabled="isReadOnly"
            class="m-input-full-width"
          />
        </FormInputRow>

        <FormInputRow label="Mã thành phần" required :error="codeError" direction="horizontal">
          <Input
            v-model="form.sc_code"
            placeholder="Nhập mã viết liền"
            :error="!!codeError"
            :disabled="isEditMode || isReadOnly"
            class="m-input-full-width"
          >
            <template #icon v-if="isEditMode">
              <!-- Icon Lock -->
              <svg
                width="16"
                height="16"
                viewBox="0 0 24 24"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M18 8H17V6C17 3.24 14.76 1 12 1C9.24 1 7 3.24 7 6V8H6C4.9 8 4 8.9 4 10V20C4 21.1 4.9 22 6 22H18C19.1 22 20 21.1 20 20V10C20 8.9 19.1 8 18 8ZM9 6C9 4.34 10.34 3 12 3C13.66 3 15 4.34 15 6V8H9V6ZM18 20H6V10H18V20ZM12 13C10.9 13 10 13.9 10 15C10 16.1 10.9 17 12 17C13.1 17 14 16.1 14 15C14 13.9 13.1 13 12 13Z"
                  fill="#9E9E9E"
                />
              </svg>
            </template>
          </Input>
        </FormInputRow>

        <FormInputRow label="Đơn vị áp dụng" required :error="orgIdsError" direction="horizontal">
          <SelectTree
            v-model="form.organization_ids"
            placeholder="Chọn đơn vị"
            :options="organizationOptions"
            node-key="OrganizationId"
            label-key="OrganizationName"
            children-key="Children"
            dropdown-width="840px"
            :disabled="isReadOnly"
            ref="organizationSelectTree"
            class="m-input-full-width"
          />
        </FormInputRow>

        <FormInputRow label="Loại thành phần" required :error="typeError" direction="horizontal">
          <Select
            v-model="form.sc_type"
            :options="typeOptions"
            placeholder="Chọn loại"
            option-label="label"
            option-value="value"
            no-auto-select
            :disabled="isReadOnly"
            :error="!!typeError"
            width="315px"
          />
        </FormInputRow>

        <FormInputRow label="Tính chất" required :error="natureError" direction="horizontal">
          <div class="nature-row-container items-center gap-12">
            <Select
              v-model="form.sc_nature"
              :options="natureOptions"
              placeholder="Chọn tính chất"
              option-label="label"
              option-value="value"
              :disabled="isReadOnly"
              :error="!!natureError"
              width="315px"
            />
            <Checkbox
              v-if="form.sc_nature === 1"
              v-model="form.sc_is_tax_deductible"
              :disabled="isReadOnly"
              label="Giảm trừ khi tính thuế"
            />
            <RadioGroup
              v-if="form.sc_nature === 0"
              v-model="form.sc_tax_status"
              :options="taxStatusOptions"
              name="tax_status"
              :disabled="isReadOnly"
              direction="horizontal"
            />
          </div>
        </FormInputRow>

        <FormInputRow label="Định mức" direction="horizontal" v-if="form.sc_nature !== 2">
          <div class="flex-column gap-8 w-100 m-input-full-width">
            <InputFormula
              v-model="form.sc_limit_expression"
              placeholder="Tự động gợi ý và tham số khi gõ"
              :suggestions="compositionOptions"
              class="m-input-full-width"
              :disabled="isReadOnly"
            />
            <div class="flex-row items-center gap-4">
              <Checkbox
                v-model="form.sc_allow_exceed_limit"
                label="Cho phép giá trị vượt quá định mức"
                :disabled="isReadOnly"
              />
              <Tooltip
                text="Nếu không tích chọn thì khi tính giá trị của thành phần lượng này mà số tiền vượt quá định mức thì chương trình sẽ tự động lấy tối đa bằng định mức đã nhập"
                font-size="12px"
              >
                <div class="m-tooltip-icon-wrapper">
                  <svg
                    width="16"
                    height="16"
                    viewBox="221 41 18 18"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <g id="Icon Placeholder_52">
                      <path
                        id="Vector_47"
                        d="m 230,47.5 h 0.008 M 229.167,50 H 230 v 3.3333 h 0.833 M 222.5,50 c 0,0.9849 0.194,1.9602 0.571,2.8701 0.377,0.91 0.929,1.7368 1.626,2.4332 0.696,0.6964 1.523,1.2489 2.433,1.6258 0.91,0.3769 1.885,0.5709 2.87,0.5709 0.985,0 1.96,-0.194 2.87,-0.5709 0.91,-0.3769 1.737,-0.9294 2.433,-1.6258 0.697,-0.6964 1.249,-1.5232 1.626,-2.4332 0.377,-0.9099 0.571,-1.8852 0.571,-2.8701 0,-1.9891 -0.79,-3.8968 -2.197,-5.3033 C 233.897,43.2902 231.989,42.5 230,42.5 c -1.989,0 -3.897,0.7902 -5.303,2.1967 C 223.29,46.1032 222.5,48.0109 222.5,50 Z"
                        stroke="#717680"
                        stroke-width="1.5"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                    </g>
                  </svg>
                </div>
              </Tooltip>
            </div>
          </div>
        </FormInputRow>

        <FormInputRow label="Kiểu giá trị" direction="horizontal">
          <Select
            v-model="form.sc_value_type"
            :options="valueTypeOptions"
            :disabled="isReadOnly || isEditMode || [0, 1].includes(form.sc_nature)"
            option-label="label"
            option-value="value"
            width="315px"
          />
        </FormInputRow>

        <FormInputRow label="Giá trị" direction="horizontal">
          <div class="value-config-container m-input-full-width">
            <div class="m-radio-group-vertical">
              <!-- Option 1: Tự động cộng tổng -->
              <Radio
                v-model="form.sc_calculation_method"
                :value="0"
                label="Tự động cộng tổng giá trị của các nhân viên"
                name="calc_method"
                :disabled="isReadOnly || isAggregationDisabled"
                :title="isAggregationDisabled ? 'Kiểu giá trị hiện tại không hỗ trợ cộng tổng' : ''"
              />

              <!-- Chi tiết Tự động cộng tổng (hàng các select, không label) -->
              <div class="m-nested-row gap-12">
                <Select
                  v-model="form.sc_aggregation_scope"
                  :options="aggregationOptions"
                  option-label="label"
                  option-value="value"
                  option-tooltip="tooltip"
                  width="315px"
                  style="flex-shrink: 0"
                  :disabled="isReadOnly || form.sc_calculation_method !== 0"
                />
                <template v-if="form.sc_calculation_method === 0">
                  <!-- Hiển thị thêm select Cấp nếu chọn Thuộc cơ cấu (value = 2) -->
                  <Select
                    v-if="form.sc_aggregation_scope === 2"
                    v-model="form.sc_organization_level"
                    placeholder="Chọn cấp"
                    :options="organizationLevelOptions"
                    option-label="label"
                    option-value="value"
                    width="120px"
                    :disabled="isReadOnly"
                    style="flex-shrink: 0"
                    no-auto-select
                  />
                  <div class="flex-column" style="flex: 1">
                    <Select
                      v-model="form.sc_composition_code"
                      placeholder="Chọn thành phần lương để cộng giá trị"
                      :options="compositionOptions"
                      option-label="ScName"
                      option-value="ScCode"
                      :disabled="isReadOnly"
                      :error="!!compCodeError"
                      width="100%"
                      no-auto-select
                    />
                    <div v-if="compCodeError" class="m-input-error-message" style="margin-left: 0">
                      {{ compCodeError }}
                    </div>
                  </div>
                </template>
              </div>

              <!-- Option 2: Tính theo công thức -->
              <Radio
                v-model="form.sc_calculation_method"
                :value="1"
                label="Tính theo công thức tự đặt"
                name="calc_method"
                :disabled="isReadOnly"
              />

              <div v-if="form.sc_calculation_method === 1" class="m-nested-row flex-column gap-12">
                <InputFormula
                  v-model="form.sc_formula_expression"
                  placeholder="Tự động gợi ý và tham số khi gõ"
                  :suggestions="compositionOptions"
                  class="m-input-full-width"
                  :disabled="isReadOnly"
                  :error="!!formulaExprError"
                />
              </div>
            </div>

            <!-- Logic phần chịu thuế/miễn thuế: Luôn hiển thị nếu là Miễn thuế 1 phần, không phụ thuộc cách tính -->
            <template
              v-if="form.sc_nature === 0 && form.sc_tax_status === TaxStatus.PartiallyExempt"
            >
              <div class="m-tax-nested-block flex-column gap-8" style="margin-top: 12px">
                <div class="m-nested-block-title">Trong đó:</div>
                <FormInputRow label="Phần chịu thuế" labelWidth="100px" direction="horizontal">
                  <div class="flex-column">
                    <InputFormula
                      v-model="form.sc_taxable_expression"
                      :placeholder="taxablePlaceholder"
                      :suggestions="compositionOptions"
                      class="m-input-full-width"
                      :disabled="isTaxableDisabled"
                      :error="!!taxableError"
                    />
                    <div v-if="taxableError" class="m-input-error-message" style="margin-left: 0">
                      {{ taxableError }}
                    </div>
                  </div>
                </FormInputRow>
                <FormInputRow label="Phần miễn thuế" labelWidth="100px" direction="horizontal">
                  <div class="flex-column">
                    <InputFormula
                      v-model="form.sc_exempt_expression"
                      :placeholder="exemptPlaceholder"
                      :suggestions="compositionOptions"
                      class="m-input-full-width"
                      :disabled="isExemptDisabled"
                      :error="!!exemptError"
                    />
                    <div v-if="exemptError" class="m-input-error-message" style="margin-left: 0">
                      {{ exemptError }}
                    </div>
                  </div>
                </FormInputRow>
              </div>
            </template>
          </div>
        </FormInputRow>

        <FormInputRow label="Mô tả" direction="horizontal">
          <InputArea
            v-model="form.sc_description"
            rows="3"
            :disabled="isReadOnly"
            class="m-input-full-width"
          />
        </FormInputRow>

        <FormInputRow label="Hiển thị trên phiếu lương" direction="horizontal">
          <RadioGroup
            v-model="form.sc_is_displayed_on_payroll"
            :options="displayPayrollOptions"
            name="display_payroll"
            :disabled="isReadOnly"
            direction="horizontal"
          />
        </FormInputRow>

        <FormInputRow label="Nguồn tạo" direction="horizontal">
          <Select
            v-model="form.sc_source"
            :options="sourceOptions"
            disabled
            option-label="label"
            option-value="value"
            width="315px"
          />
        </FormInputRow>

        <FormInputRow v-if="isEditMode" label="Trạng thái" direction="horizontal">
          <RadioGroup
            v-model="form.sc_status"
            :options="scStatusOptions"
            name="sc_status"
            :disabled="isReadOnly"
            direction="horizontal"
          />
        </FormInputRow>
      </FormInputSection>
    </div>
  </ThePageContainer>
</template>

<script setup>
import ThePageContainer from '@/layout/ThePageContainer.vue'
import ButtonGroup from '@/components/controls/buttons/ButtonGroup.vue'
import Button from '@/components/controls/buttons/Button.vue'
import ButtonIcon from '@/components/controls/buttons/ButtonIcon.vue'
import ConfirmationModal from '@/components/base/ConfirmationModal.vue'
import FormInputSection from '@/components/form/FormInputSection.vue'
import FormInputRow from '@/components/form/FormInputRow.vue'
import Input from '@/components/controls/inputs/Input.vue'
import SelectTree from '@/components/controls/selects/SelectTree.vue'
import Select from '@/components/controls/selects/Select.vue'
import Checkbox from '@/components/controls/checkboxes/Checkbox.vue'
import RadioGroup from '@/components/controls/radio-buttons/RadioGroup.vue'
import Radio from '@/components/controls/radio-buttons/Radio.vue'
import InputFormula from '@/components/controls/inputs/InputFormula.vue'
import Tooltip from '@/components/base/Tooltip.vue'
import InputArea from '@/components/controls/inputs/InputArea.vue'
import PencilIcon from '@/components/icons/PencilIcon.vue'
import MoreIcon from '@/components/icons/MoreIcon.vue'

import useSalaryCompositionForm from './useSalaryCompositionForm.js'

const props = defineProps({
  compositionId: {
    type: String,
    default: null,
  },
  duplicateId: {
    type: String,
    default: null,
  },
})

const emit = defineEmits(['close', 'save', 'duplicate', 'error-notification'])

const {
  form,
  isEditMode,
  isReadOnly,
  isDeleteModalVisible,
  formTitle,
  moreActions,
  nameError,
  orgIdsError,
  typeError,
  natureError,
  compCodeError,
  formulaExprError,
  taxableError,
  exemptError,
  codeError,
  isUnsavedChangesModalVisible,
  organizationOptions,
  compositionOptions,
  typeOptions,
  natureOptions,
  valueTypeOptions,
  taxStatusOptions,
  aggregationOptions,
  organizationLevelOptions,
  displayPayrollOptions,
  scStatusOptions,
  sourceOptions,
  isAggregationDisabled,
  isTaxableDisabled,
  isExemptDisabled,
  taxablePlaceholder,
  exemptPlaceholder,
  handleSave,
  handleCancelEdit,
  handleClose,
  confirmExit,
  confirmDelete,
  TaxStatus,
  SalaryCompositionSource,
} = useSalaryCompositionForm(props, { emit })
</script>

<style scoped>
.m-salary-form-layout {
  display: flex;
  flex-direction: column;
  height: 100%;
  width: 100%;
  overflow: hidden;
}

.m-salary-form-main {
  flex: 1;
  min-height: 0;
}

.form-scroll-content {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 0px; /* Giảm gap để kiểm soát chiều cao row tốt hơn thông qua margin/padding của component con */
  padding: 40px;
}

.m-input-full-width {
  width: 100% !important;
  max-width: 840px !important;
}

.nature-row-container {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
}

@media (max-width: 1024px) {
  .nature-row-container {
    flex-direction: column;
    align-items: flex-start !important;
  }
}

.m-textarea {
  width: 100%;
  padding: 8px 12px;
  border-radius: 4px;
  border: 1px solid #e0e0e0;
  outline: none;
  font-family: inherit;
}

.m-textarea:focus {
  border-color: #34b057;
}

.m-radio-group,
.m-radio-group-horizontal,
.m-radio-group-vertical {
  display: flex;
  gap: 20px;
}

.value-config-container,
.m-radio-group-vertical {
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.flex-row {
  display: flex;
  flex-direction: row;
}

.items-center {
  align-items: center;
}

.gap-4 {
  gap: 4px;
}

.flex-column {
  display: flex;
  flex-direction: column;
}

.gap-8 {
  gap: 8px;
}

.gap-12 {
  gap: 12px;
}

.m-nested-row {
  display: flex;
  gap: 8px;
  width: 100%;
}

.m-nested-label {
  font-size: 13px;
  color: #111;
  margin-bottom: -4px;
}

.m-tooltip-icon-wrapper {
  width: 36px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  cursor: pointer;
  transition: background-color 0.2s;
}
.m-tooltip-icon-wrapper:hover {
  background-color: var(--item-hover, #f2f2f2);
}

.m-form-footer {
  height: 56px;
  padding: 0 16px;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  background-color: var(--bg-app);
  flex-shrink: 0;
}

.flex-grow {
  flex-grow: 1;
}

.m-nested-block-title {
  font-size: 13px;
  color: var(--color-text-main);
  margin: 8px 0;
}

.m-input-error-message {
  color: var(--color-error);
  font-size: 12px;
  margin-top: 4px;
}
</style>
