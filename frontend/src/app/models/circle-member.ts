export interface CircleMember {
  id: number | string;
  circleId: number | string;
  memberId: number | string;
  memberName?: string;
  orderNumber: number;
  joinedAt?: string;
}

export interface AddCircleMemberRequest {
  circleId: number;
  memberId: number;
  orderNumber: number;
}