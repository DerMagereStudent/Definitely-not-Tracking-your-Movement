/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { IdentitySignupComponent } from './identity-signup.component';

describe('IdentitySignupComponent', () => {
  let component: IdentitySignupComponent;
  let fixture: ComponentFixture<IdentitySignupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IdentitySignupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IdentitySignupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
