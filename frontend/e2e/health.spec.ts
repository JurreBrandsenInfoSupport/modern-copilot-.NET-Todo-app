import { test, expect } from '@playwright/test'

test.describe('Health', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login')
    await page.getByLabel('Username').fill('demo')
    await page.getByRole('button', { name: 'Sign In' }).click()
    await expect(page).toHaveURL('/')
    await page.getByRole('link', { name: /health/i }).click()
    await expect(page).toHaveURL('/health')
  })

  test('should display health check page', async ({ page }) => {
    await expect(page.getByText('Health Check')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Refresh' })).toBeVisible()
  })

  test('should show healthy status', async ({ page }) => {
    await expect(page.getByText('System Healthy')).toBeVisible({ timeout: 10000 })
    await expect(page.getByText('/health')).toBeVisible()
    await expect(page.getByText('200')).toBeVisible()
  })
})
