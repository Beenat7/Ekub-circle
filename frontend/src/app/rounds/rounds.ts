import { Component, OnInit } from '@angular/core';
import { Round } from '../models/round.model';
import { RoundService } from '../services/round';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-rounds',
  imports: [DatePipe],
  templateUrl: './rounds.html',
  styleUrl: './rounds.scss'
})
export class Rounds implements OnInit {
  rounds: Round[] = [];
  loading = true;
  error: string | null = null;

  constructor(private roundService: RoundService) {}

  ngOnInit(): void {
    this.roundService.getRounds().subscribe({
      next: (rounds) => {
        this.rounds = rounds;
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load rounds:', error);
        this.error = 'Could not load rounds. Please try again.';
        this.loading = false;
      }
    });
  }
}