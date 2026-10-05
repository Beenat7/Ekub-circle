import { Component, computed, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  readonly currentUrl = signal('');

  constructor(public readonly router: Router) {
    this.currentUrl.set(this.router.url);

    this.router.events.subscribe(() => {
      this.currentUrl.set(this.router.url);
    });
  }

  readonly showSidebar = computed(
    () => !['/login', '/register'].includes(this.currentUrl())
  );

  logout(): void {
    this.router.navigate(['/login']);
  }
}