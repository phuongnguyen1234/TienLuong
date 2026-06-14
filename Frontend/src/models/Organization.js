/**
 * Đại diện cho một đơn vị công tác.
 */
class Organization {
  constructor(organizationId, organizationName, organizationParentId, children = []) {
    this.organizationId = organizationId
    this.organizationName = organizationName
    this.organizationParentId = organizationParentId
    this.children = children
  }
}

export default Organization
