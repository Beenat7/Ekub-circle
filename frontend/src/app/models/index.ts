export interface User {
  id: number;
  username: string;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  phoneNumber?: string;
  createdAt?: string;
}

export interface Circle {
  id: number;
  name: string;
  contributionAmount: number;
  maxMembers: number;
  currentMemberCount?: number;
  contributionIntervalDays: number;
  status: string;
  organizerId: number;
  organizerName?: string;
  createdAt: string;
}

export interface CreateCirclePayload {
  name: string;
  contributionAmount: number;
  maxMembers: number;
  contributionIntervalDays: number;
  organizerId: number;
}

export interface JoinCircleResponse {
  circleMemberId: number;
  circleId: number;
  memberId: number;
  orderNumber: number;
  joinedAt: string;
}
