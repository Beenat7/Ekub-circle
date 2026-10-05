import { Injectable } from '@angular/core';
import { Round } from '../models/round.model';

@Injectable({
  providedIn: 'root'
})
export class RoundService {

  private rounds: Round[] = [
    {
      id: 1,
      circleId: 1,
      roundNumber: 1,
      startDate: '2026-10-01',
      endDate: '2026-10-31',
      recipientId: 101,
      status: 'completed'
    },
    {
      id: 2,
      circleId: 1,
      roundNumber: 2,
      startDate: '2026-11-01',
      endDate: '2026-11-30',
      recipientId: 102,
      status: 'active'
    },
    {
      id: 3,
      circleId: 1,
      roundNumber: 3,
      startDate: '2026-12-01',
      endDate: '2026-12-31',
      recipientId: 103,
      status: 'upcoming'
    }
  ];

  getRounds(): Round[] {
    return this.rounds;
  }

  getRoundById(id: number): Round | undefined {
    return this.rounds.find(round => round.id === id);
  }

  getRoundsByCircle(circleId: number): Round[] {
    return this.rounds.filter(round => round.circleId === circleId);
  }
}