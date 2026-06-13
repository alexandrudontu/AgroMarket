import { TestBed } from '@angular/core/testing';
import { HttpHandlerFn, HttpRequest, HttpResponse } from '@angular/common/http';
import { firstValueFrom, of } from 'rxjs';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';

import { authInterceptor } from './auth-interceptor';

describe('authInterceptor', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({});
    localStorage.clear();
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should add Authorization header when token exists', async () => {
    localStorage.setItem('token', 'test-token');

    const request = new HttpRequest('GET', 'https://localhost:7183/api/products');

    let handledRequest: HttpRequest<unknown> | undefined;

    const next: HttpHandlerFn = (req) => {
      handledRequest = req;
      return of(new HttpResponse({ status: 200, body: [] }));
    };

    await firstValueFrom(
      TestBed.runInInjectionContext(() => authInterceptor(request, next))
    );

    expect(handledRequest).toBeDefined();
    expect(handledRequest!.headers.get('Authorization')).toBe('Bearer test-token');
  });

  it('should not add Authorization header when token does not exist', async () => {
    const request = new HttpRequest('GET', 'https://localhost:7183/api/products');

    let handledRequest: HttpRequest<unknown> | undefined;

    const next: HttpHandlerFn = (req) => {
      handledRequest = req;
      return of(new HttpResponse({ status: 200, body: [] }));
    };

    await firstValueFrom(
      TestBed.runInInjectionContext(() => authInterceptor(request, next))
    );

    expect(handledRequest).toBeDefined();
    expect(handledRequest!.headers.has('Authorization')).toBe(false);
  });
});