import { Component, OnInit } from '@angular/core';
import { Round } from '../models/round.model';
import { RoundService } from '../services/round';

@Component({
  selector: 'app-rounds',
  imports: [],
  templateUrl: './rounds.html',
  styleUrl: './rounds.scss'
})
export class Rounds implements OnInit {

  rounds: Round[] = [];

  constructor(private roundService: RoundService) {}

  ngOnInit(): void {
    this.rounds = this.roundService.getRounds();
  }
}