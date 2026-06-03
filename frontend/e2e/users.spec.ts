import { test, expect } from '@playwright/test'

test.describe('Users', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login')
    await page.getByLabel('Username').fill('demo')
    await page.getByRole('button', { name: 'Sign In' }).click()
    await expect(page).toHaveURL('/')
    await page.getByRole('link', { name: /users/i }).click()
    await expect(page).toHaveURL('/users')
  })

  test('should display users page with demo user', async ({ page }) => {
    await expect(page.getByText('Register New User')).toBeVisible()
    await expect(page.getByRole('main').getByText('demo')).toBeVisible()
  })

  test('should register a new user', async ({ page }) => {
    const uniqueName = `testuser_${Date.now()}`
    await page.getByPlaceholder('Username').fill(uniqueName)
    await page.getByRole('button', { name: 'Register' }).click()

    // New user should appear in the list
    await expect(page.getByText(uniqueName)).toBeVisible({ timeout: 5000 })
  })
})
